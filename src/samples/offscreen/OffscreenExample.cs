// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: offscreen/offscreen.cpp

/*
* Vulkan Example - Offscreen rendering using a separate framebuffer
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vulkan;
using static Vulkan.VulkanNative;
using static Vulkan.RawConstants;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Veldrid.Sdl2;
using Veldrid;

namespace Vk.Samples
{
    public unsafe class OffscreenExample : VulkanExampleBase
    {
        private const uint VERTEX_BUFFER_BIND_ID = 0;

        // Offscreen frame buffer properties
        private const int FB_DIM = 512;
        private const VkFormat FB_COLOR_FORMAT = VK_FORMAT_R8G8B8A8_UNORM;

        bool debugDisplay = false;

        vksTexture2D textures_colorMap = new vksTexture2D();

        // Vertex layout for the models
        vksVertexLayout vertexLayout = new vksVertexLayout(
          VertexComponent.VERTEX_COMPONENT_POSITION,
           VertexComponent.VERTEX_COMPONENT_UV,
           VertexComponent.VERTEX_COMPONENT_COLOR,
           VertexComponent.VERTEX_COMPONENT_NORMAL);

        vksModel models_example = new vksModel();
        vksModel models_quad = new vksModel();
        vksModel models_plane = new vksModel();

        VkPipelineVertexInputStateCreateInfo vertices_inputState;
        NativeList<VkVertexInputBindingDescription> vertices_bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
        NativeList<VkVertexInputAttributeDescription> vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();

        vksBuffer uniformBuffers_vsShared = new vksBuffer();
        vksBuffer uniformBuffers_vsMirror = new vksBuffer();
        vksBuffer uniformBuffers_vsOffScreen = new vksBuffer();
        vksBuffer uniformBuffers_vsDebugQuad = new vksBuffer();

        struct UBO
        {
            public Matrix4x4 projection;
            public Matrix4x4 model;
            public Vector4 lightPos;
        }

        UBO uboShared = new UBO() { lightPos = new Vector4(0.0f, 0.0f, 0.0f, 1.0f) };

        VkPipeline pipelines_debug;
        VkPipeline pipelines_shaded;
        VkPipeline pipelines_shadedOffscreen;
        VkPipeline pipelines_mirror;

        VkPipelineLayout pipelineLayouts_textured;
        VkPipelineLayout pipelineLayouts_shaded;

        VkDescriptorSet descriptorSets_offscreen;
        VkDescriptorSet descriptorSets_mirror;
        VkDescriptorSet descriptorSets_model;
        VkDescriptorSet descriptorSets_debugQuad;

        VkDescriptorSetLayout descriptorSetLayouts_textured;
        VkDescriptorSetLayout descriptorSetLayouts_shaded;

        // Framebuffer for offscreen rendering
        struct FrameBufferAttachment
        {
            public VkImage image;
            public VkDeviceMemory mem;
            public VkImageView view;
        };

        struct OffscreenPass
        {
            public int width, height;
            public VkFramebuffer frameBuffer;
            public FrameBufferAttachment color, depth;
            public VkRenderPass renderPass;
            public VkSampler sampler;
            public VkDescriptorImageInfo descriptor;
            public VkCommandBuffer commandBuffer;
            // Semaphore used to synchronize between offscreen and final scene render pass
            public VkSemaphore semaphore;
        }

        OffscreenPass offscreenPass;

        Vector3 meshPos = new Vector3(0.0f, -1.5f, 0.0f);
        Vector3 meshRot = new Vector3(0.0f);


        public OffscreenExample()
        {
            zoom = -6.0f;
            rotation = new Vector3(-2.5f, 0.0f, 0.0f);
            cameraPos = new Vector3(0.0f, 1.0f, 0.0f);
            timerSpeed *= 0.25f;
            enableTextOverlay = true;
            title = "Vulkan Example - Offscreen rendering";
            var ef = enabledFeatures;
            ef.shaderClipDistance = VK_TRUE;
            enabledFeatures = ef;
        }

        public void Dispose()
        {
            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class

            // Textures
            textures_colorMap.destroy();

            // Frame buffer

            // Color attachment
            vkDestroyImageView(device, offscreenPass.color.view, null);
            vkDestroyImage(device, offscreenPass.color.image, null);
            vkFreeMemory(device, offscreenPass.color.mem, null);

            // Depth attachment
            vkDestroyImageView(device, offscreenPass.depth.view, null);
            vkDestroyImage(device, offscreenPass.depth.image, null);
            vkFreeMemory(device, offscreenPass.depth.mem, null);

            vkDestroyRenderPass(device, offscreenPass.renderPass, null);
            vkDestroySampler(device, offscreenPass.sampler, null);
            vkDestroyFramebuffer(device, offscreenPass.frameBuffer, null);

            vkDestroyPipeline(device, pipelines_debug, null);
            vkDestroyPipeline(device, pipelines_shaded, null);
            vkDestroyPipeline(device, pipelines_shadedOffscreen, null);
            vkDestroyPipeline(device, pipelines_mirror, null);

            vkDestroyPipelineLayout(device, pipelineLayouts_textured, null);
            vkDestroyPipelineLayout(device, pipelineLayouts_shaded, null);

            vkDestroyDescriptorSetLayout(device, descriptorSetLayouts_shaded, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayouts_textured, null);

            // Models
            models_example.destroy();
            models_quad.destroy();
            models_plane.destroy();

            // Uniform buffers
            uniformBuffers_vsShared.destroy();
            uniformBuffers_vsMirror.destroy();
            uniformBuffers_vsOffScreen.destroy();
            uniformBuffers_vsDebugQuad.destroy();

            vkFreeCommandBuffers(device, cmdPool, 1, ref offscreenPass.commandBuffer);
            vkDestroySemaphore(device, offscreenPass.semaphore, null);
        }

        // Setup the offscreen framebuffer for rendering the mirrored scene
        // The color attachment of this framebuffer will then be used to sample from in the fragment shader of the final pass
        void prepareOffscreen()
        {
            offscreenPass.width = FB_DIM;
            offscreenPass.height = FB_DIM;

            // Find a suitable depth format
            VkFormat fbDepthFormat;
            VkBool32 validDepthFormat = Tools.getSupportedDepthFormat(physicalDevice, &fbDepthFormat);
            Debug.Assert(validDepthFormat);

            // Color attachment
            VkImageCreateInfo image = Initializers.imageCreateInfo();
            image.imageType = VK_IMAGE_TYPE_2D;
            image.format = FB_COLOR_FORMAT;
            image.extent.width = (uint)offscreenPass.width;
            image.extent.height = (uint)offscreenPass.height;
            image.extent.depth = 1;
            image.mipLevels = 1;
            image.arrayLayers = 1;
            image.samples = VkSampleCountFlags.Count1;
            image.tiling = VK_IMAGE_TILING_OPTIMAL;
            // We will sample directly from the color attachment
            image.usage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT | VK_IMAGE_USAGE_SAMPLED_BIT;

            VkMemoryAllocateInfo memAlloc = Initializers.memoryAllocateInfo();
            VkMemoryRequirements memReqs;

            Util.CheckResult(vkCreateImage(device, &image, null, out offscreenPass.color.image));
            vkGetImageMemoryRequirements(device, offscreenPass.color.image, &memReqs);
            memAlloc.allocationSize = memReqs.size;
            memAlloc.memoryTypeIndex = vulkanDevice.getMemoryType(memReqs.memoryTypeBits, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);
            Util.CheckResult(vkAllocateMemory(device, &memAlloc, null, out offscreenPass.color.mem));
            Util.CheckResult(vkBindImageMemory(device, offscreenPass.color.image, offscreenPass.color.mem, 0));

            VkImageViewCreateInfo colorImageView = Initializers.imageViewCreateInfo();
            colorImageView.viewType = VK_IMAGE_VIEW_TYPE_2D;
            colorImageView.format = FB_COLOR_FORMAT;
            colorImageView.subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color);
            colorImageView.image = offscreenPass.color.image;
            Util.CheckResult(vkCreateImageView(device, &colorImageView, null, out offscreenPass.color.view));

            // Create sampler to sample from the attachment in the fragment shader
            VkSamplerCreateInfo samplerInfo = Initializers.samplerCreateInfo();
            samplerInfo.magFilter = VK_FILTER_LINEAR;
            samplerInfo.minFilter = VK_FILTER_LINEAR;
            samplerInfo.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
            samplerInfo.addressModeU = VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE;
            samplerInfo.addressModeV = samplerInfo.addressModeU;
            samplerInfo.addressModeW = samplerInfo.addressModeU;
            samplerInfo.mipLodBias = 0.0f;
            samplerInfo.maxAnisotropy = 0;
            samplerInfo.minLod = 0.0f;
            samplerInfo.maxLod = 1.0f;
            samplerInfo.borderColor = VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE;
            Util.CheckResult(vkCreateSampler(device, &samplerInfo, null, out offscreenPass.sampler));

            // Depth stencil attachment
            image.format = fbDepthFormat;
            image.usage = VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT;

            Util.CheckResult(vkCreateImage(device, &image, null, out offscreenPass.depth.image));
            vkGetImageMemoryRequirements(device, offscreenPass.depth.image, &memReqs);
            memAlloc.allocationSize = memReqs.size;
            memAlloc.memoryTypeIndex = vulkanDevice.getMemoryType(memReqs.memoryTypeBits, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);
            Util.CheckResult(vkAllocateMemory(device, &memAlloc, null, out offscreenPass.depth.mem));
            Util.CheckResult(vkBindImageMemory(device, offscreenPass.depth.image, offscreenPass.depth.mem, 0));

            VkImageViewCreateInfo depthStencilView = Initializers.imageViewCreateInfo();
            depthStencilView.viewType = VK_IMAGE_VIEW_TYPE_2D;
            depthStencilView.format = fbDepthFormat;
            depthStencilView.flags = 0;
            depthStencilView.subresourceRange = new VkImageSubresourceRange();
            depthStencilView.subresourceRange.aspectMask = VK_IMAGE_ASPECT_DEPTH_BIT | VK_IMAGE_ASPECT_STENCIL_BIT;
            depthStencilView.subresourceRange.baseMipLevel = 0;
            depthStencilView.subresourceRange.levelCount = 1;
            depthStencilView.subresourceRange.baseArrayLayer = 0;
            depthStencilView.subresourceRange.layerCount = 1;
            depthStencilView.image = offscreenPass.depth.image;
            Util.CheckResult(vkCreateImageView(device, &depthStencilView, null, out offscreenPass.depth.view));

            // Create a separate render pass for the offscreen rendering as it may differ from the one used for scene rendering

            FixedArray2<VkAttachmentDescription> attchmentDescriptions = new FixedArray2<VkAttachmentDescription>();
            // Color attachment
            attchmentDescriptions.First.format = FB_COLOR_FORMAT;
            attchmentDescriptions.First.samples = VkSampleCountFlags.Count1;
            attchmentDescriptions.First.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;
            attchmentDescriptions.First.storeOp = VK_ATTACHMENT_STORE_OP_STORE;
            attchmentDescriptions.First.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
            attchmentDescriptions.First.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
            attchmentDescriptions.First.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
            attchmentDescriptions.First.finalLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            // Depth attachment
            attchmentDescriptions.Second.format = fbDepthFormat;
            attchmentDescriptions.Second.samples = VkSampleCountFlags.Count1;
            attchmentDescriptions.Second.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;
            attchmentDescriptions.Second.storeOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
            attchmentDescriptions.Second.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
            attchmentDescriptions.Second.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
            attchmentDescriptions.Second.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
            attchmentDescriptions.Second.finalLayout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL;

            VkAttachmentReference colorReference = new VkAttachmentReference() { attachment = 0, layout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL };
            VkAttachmentReference depthReference = new VkAttachmentReference() { attachment = 1, layout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL };

            VkSubpassDescription subpassDescription = new VkSubpassDescription();
            subpassDescription.pipelineBindPoint = VK_PIPELINE_BIND_POINT_GRAPHICS;
            subpassDescription.colorAttachmentCount = 1;
            subpassDescription.pColorAttachments = &colorReference;
            subpassDescription.pDepthStencilAttachment = &depthReference;

            // Use subpass dependencies for layout transitions
            FixedArray2<VkSubpassDependency> dependencies = new FixedArray2<VkSubpassDependency>();

            dependencies.First.srcSubpass = VK_SUBPASS_EXTERNAL;
            dependencies.First.dstSubpass = 0;
            dependencies.First.srcStageMask = VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT;
            dependencies.First.dstStageMask = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT;
            dependencies.First.srcAccessMask = VK_ACCESS_MEMORY_READ_BIT;
            dependencies.First.dstAccessMask = VK_ACCESS_COLOR_ATTACHMENT_READ_BIT | VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT;
            dependencies.First.dependencyFlags = VK_DEPENDENCY_BY_REGION_BIT;

            dependencies.Second.srcSubpass = 0;
            dependencies.Second.dstSubpass = VK_SUBPASS_EXTERNAL;
            dependencies.Second.srcStageMask = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT;
            dependencies.Second.dstStageMask = VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT;
            dependencies.Second.srcAccessMask = VK_ACCESS_COLOR_ATTACHMENT_READ_BIT | VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT;
            dependencies.Second.dstAccessMask = VK_ACCESS_MEMORY_READ_BIT;
            dependencies.Second.dependencyFlags = VK_DEPENDENCY_BY_REGION_BIT;

            // Create the actual renderpass
            VkRenderPassCreateInfo renderPassInfo = VkRenderPassCreateInfo.New();
            renderPassInfo.attachmentCount = attchmentDescriptions.Count;
            renderPassInfo.pAttachments = &attchmentDescriptions.First;
            renderPassInfo.subpassCount = 1;
            renderPassInfo.pSubpasses = &subpassDescription;
            renderPassInfo.dependencyCount = dependencies.Count;
            renderPassInfo.pDependencies = &dependencies.First;

            Util.CheckResult(vkCreateRenderPass(device, &renderPassInfo, null, out offscreenPass.renderPass));

            FixedArray2<VkImageView> attachments = new FixedArray2<VkImageView>(
                offscreenPass.color.view,
                offscreenPass.depth.view);

            VkFramebufferCreateInfo fbufCreateInfo = Initializers.framebufferCreateInfo();
            fbufCreateInfo.renderPass = offscreenPass.renderPass;
            fbufCreateInfo.attachmentCount = 2;
            fbufCreateInfo.pAttachments = &attachments.First;
            fbufCreateInfo.width = (uint)offscreenPass.width;
            fbufCreateInfo.height = (uint)offscreenPass.height;
            fbufCreateInfo.layers = 1;

            Util.CheckResult(vkCreateFramebuffer(device, &fbufCreateInfo, null, out offscreenPass.frameBuffer));

            // Fill a descriptor for later use in a descriptor set 
            offscreenPass.descriptor.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            offscreenPass.descriptor.imageView = offscreenPass.color.view;
            offscreenPass.descriptor.sampler = offscreenPass.sampler;
        }

        // Sets up the command buffer that renders the scene to the offscreen frame buffer
        void buildOffscreenCommandBuffer()
        {
            if (offscreenPass.commandBuffer == NullHandle)
            {
                offscreenPass.commandBuffer = createCommandBuffer(VK_COMMAND_BUFFER_LEVEL_PRIMARY, false);
            }
            if (offscreenPass.semaphore == 0)
            {
                // Create a semaphore used to synchronize offscreen rendering and usage
                VkSemaphoreCreateInfo semaphoreCreateInfo = Initializers.semaphoreCreateInfo();
                Util.CheckResult(vkCreateSemaphore(device, &semaphoreCreateInfo, null, out offscreenPass.semaphore));
            }

            VkCommandBufferBeginInfo cmdBufInfo = Initializers.commandBufferBeginInfo();

            FixedArray2<VkClearValue> clearValues = new FixedArray2<VkClearValue>();
            clearValues.First.color = new VkClearColorValue(0.0f, 0.0f, 0.0f, 0.0f);
            clearValues.Second.depthStencil = new VkClearDepthStencilValue(1.0f, 0);

            VkRenderPassBeginInfo renderPassBeginInfo = Initializers.renderPassBeginInfo();
            renderPassBeginInfo.renderPass = offscreenPass.renderPass;
            renderPassBeginInfo.framebuffer = offscreenPass.frameBuffer;
            renderPassBeginInfo.renderArea.extent.width = (uint)offscreenPass.width;
            renderPassBeginInfo.renderArea.extent.height = (uint)offscreenPass.height;
            renderPassBeginInfo.clearValueCount = 2;
            renderPassBeginInfo.pClearValues = &clearValues.First;

            Util.CheckResult(vkBeginCommandBuffer(offscreenPass.commandBuffer, &cmdBufInfo));

            vkCmdBeginRenderPass(offscreenPass.commandBuffer, &renderPassBeginInfo, VK_SUBPASS_CONTENTS_INLINE);

            VkViewport viewport = Initializers.viewport((float)offscreenPass.width, (float)offscreenPass.height, 0.0f, 1.0f);
            vkCmdSetViewport(offscreenPass.commandBuffer, 0, 1, &viewport);

            VkRect2D scissor = Initializers.rect2D((uint)offscreenPass.width, (uint)offscreenPass.height, 0, 0);
            vkCmdSetScissor(offscreenPass.commandBuffer, 0, 1, &scissor);

            ulong offsets = 0;

            // Mirrored scene
            vkCmdBindDescriptorSets(offscreenPass.commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayouts_shaded, 0, 1, ref descriptorSets_offscreen, 0, null);
            vkCmdBindPipeline(offscreenPass.commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_shadedOffscreen);
            vkCmdBindVertexBuffers(offscreenPass.commandBuffer, VERTEX_BUFFER_BIND_ID, 1, ref models_example.vertices.buffer, &offsets);
            vkCmdBindIndexBuffer(offscreenPass.commandBuffer, models_example.indices.buffer, 0, VK_INDEX_TYPE_UINT32);
            vkCmdDrawIndexed(offscreenPass.commandBuffer, models_example.indexCount, 1, 0, 0, 0);

            vkCmdEndRenderPass(offscreenPass.commandBuffer);

            Util.CheckResult(vkEndCommandBuffer(offscreenPass.commandBuffer));
        }

        void reBuildCommandBuffers()
        {
            if (!checkCommandBuffers())
            {
                destroyCommandBuffers();
                createCommandBuffers();
            }
            buildCommandBuffers();
        }

        protected override void buildCommandBuffers()
        {
            VkCommandBufferBeginInfo cmdBufInfo = Initializers.commandBufferBeginInfo();

            FixedArray2<VkClearValue> clearValues = new FixedArray2<VkClearValue>();
            clearValues.First.color = defaultClearColor;
            clearValues.Second.depthStencil = new VkClearDepthStencilValue { depth = 1.0f, stencil = 0 };

            VkRenderPassBeginInfo renderPassBeginInfo = Initializers.renderPassBeginInfo();
            renderPassBeginInfo.renderPass = renderPass;
            renderPassBeginInfo.renderArea.offset.x = 0;
            renderPassBeginInfo.renderArea.offset.y = 0;
            renderPassBeginInfo.renderArea.extent.width = width;
            renderPassBeginInfo.renderArea.extent.height = height;
            renderPassBeginInfo.clearValueCount = 2;
            renderPassBeginInfo.pClearValues = &clearValues.First;

            for (int i = 0; i < drawCmdBuffers.Count; ++i)
            {
                // Set target frame buffer
                renderPassBeginInfo.framebuffer = frameBuffers[i];

                Util.CheckResult(vkBeginCommandBuffer(drawCmdBuffers[i], &cmdBufInfo));

                vkCmdBeginRenderPass(drawCmdBuffers[i], &renderPassBeginInfo, VK_SUBPASS_CONTENTS_INLINE);

                VkViewport viewport = Initializers.viewport((float)width, (float)height, 0.0f, 1.0f);
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);

                VkRect2D scissor = Initializers.rect2D(width, height, 0, 0);
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, &scissor);

                ulong offsets = 0;

                if (debugDisplay)
                {
                    vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayouts_textured, 0, 1, ref descriptorSets_debugQuad, 0, null);
                    vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_debug);
                    vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_quad.vertices.buffer, &offsets);
                    vkCmdBindIndexBuffer(drawCmdBuffers[i], models_quad.indices.buffer, 0, VK_INDEX_TYPE_UINT32);
                    vkCmdDrawIndexed(drawCmdBuffers[i], models_quad.indexCount, 1, 0, 0, 0);
                }

                // Scene
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_debug);

                // Reflection plane
                vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayouts_textured, 0, 1, ref descriptorSets_mirror, 0, null);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_mirror);

                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_plane.vertices.buffer, &offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], models_plane.indices.buffer, 0, VK_INDEX_TYPE_UINT32);
                vkCmdDrawIndexed(drawCmdBuffers[i], models_plane.indexCount, 1, 0, 0, 0);

                // Model
                vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayouts_shaded, 0, 1, ref descriptorSets_model, 0, null);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_shaded);

                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_example.vertices.buffer, &offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], models_example.indices.buffer, 0, VK_INDEX_TYPE_UINT32);
                vkCmdDrawIndexed(drawCmdBuffers[i], models_example.indexCount, 1, 0, 0, 0);

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void loadAssets()
        {
            // The original sample uses "plane.obj". The version of Assimp being used here does not properly load this file for some reason.
            // I've converted it into a Collada file which is loaded correctly.
            models_plane.loadFromFile(getAssetPath() + "models/plane2.dae", vertexLayout, 0.5f, vulkanDevice, queue);
            models_example.loadFromFile(getAssetPath() + "models/chinesedragon.dae", vertexLayout, 0.3f, vulkanDevice, queue);

            // Textures
            if (vulkanDevice.features.textureCompressionBC)
            {
                textures_colorMap.loadFromFile(getAssetPath() + "textures/darkmetal_bc3_unorm.ktx", VK_FORMAT_BC3_UNORM_BLOCK, vulkanDevice, queue);
            }
            else if (vulkanDevice.features.textureCompressionASTC_LDR)
            {
                textures_colorMap.loadFromFile(getAssetPath() + "textures/darkmetal_astc_8x8_unorm.ktx", VK_FORMAT_ASTC_8x8_UNORM_BLOCK, vulkanDevice, queue);
            }
            else if (vulkanDevice.features.textureCompressionETC2)
            {
                textures_colorMap.loadFromFile(getAssetPath() + "textures/darkmetal_etc2_unorm.ktx", VK_FORMAT_ETC2_R8G8B8_UNORM_BLOCK, vulkanDevice, queue);
            }
            else
            {
                throw new InvalidOperationException("Device does not support any compressed texture format!");
            }
        }

        struct Vertex
        {
            public Vector3 pos;
            public Vector2 uv;
            public Vector3 col;
            public Vector3 normal;
        }

        void generateQuad()
        {
            // Setup vertices for a single uv-mapped quad

            Vector3 defaultColor = new Vector3(1, 1, 1);
            Vector3 defaultNormal = new Vector3(0, 0, 1);

            FixedArray4<Vertex> vertexBuffer = new FixedArray4<Vertex>(
                new Vertex { pos = new Vector3(1, 1, 0), uv = new Vector2(1, 1), col = defaultColor, normal = defaultNormal },
                new Vertex { pos = new Vector3(0, 1, 0), uv = new Vector2(0, 1), col = defaultColor, normal = defaultNormal },
                new Vertex { pos = new Vector3(0, 0, 0), uv = new Vector2(0, 0), col = defaultColor, normal = defaultNormal },
                new Vertex { pos = new Vector3(1, 0, 0), uv = new Vector2(1, 0), col = defaultColor, normal = defaultNormal });

            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                (ulong)(vertexBuffer.Count * sizeof(Vertex)),
                out models_quad.vertices.buffer,
                out models_quad.vertices.memory,
                &vertexBuffer.First));

            // Setup indices
            FixedArray6<uint> indexBuffer = new FixedArray6<uint>(0, 1, 2, 2, 3, 0);
            models_quad.indexCount = indexBuffer.Count;

            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                indexBuffer.Count * sizeof(uint),
                out models_quad.indices.buffer,
                out models_quad.indices.memory,
                &indexBuffer.First));

            models_quad.device = device;
        }

        void setupVertexDescriptions()
        {
            // Binding description
            vertices_bindingDescriptions.Count = 1;
            vertices_bindingDescriptions[0] =
                Initializers.vertexInputBindingDescription(
                    VERTEX_BUFFER_BIND_ID,
                    vertexLayout.stride(),
                    VK_VERTEX_INPUT_RATE_VERTEX);

            // Attribute descriptions
            vertices_attributeDescriptions.Count = 4;
            // Location 0 : Position
            vertices_attributeDescriptions[0] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    0,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    0);
            // Location 1 : Texture coordinates
            vertices_attributeDescriptions[1] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    1,
                    VK_FORMAT_R32G32_SFLOAT,
                    sizeof(float) * 3);
            // Location 2 : Color
            vertices_attributeDescriptions[2] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    2,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    sizeof(float) * 5);
            // Location 3 : Normal
            vertices_attributeDescriptions[3] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    3,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    sizeof(float) * 8);

            vertices_inputState = Initializers.pipelineVertexInputStateCreateInfo();
            vertices_inputState.vertexBindingDescriptionCount = vertices_bindingDescriptions.Count;
            vertices_inputState.pVertexBindingDescriptions = (VkVertexInputBindingDescription*)vertices_bindingDescriptions.Data;
            vertices_inputState.vertexAttributeDescriptionCount = vertices_attributeDescriptions.Count;
            vertices_inputState.pVertexAttributeDescriptions = (VkVertexInputAttributeDescription*)vertices_attributeDescriptions.Data;
        }

        void setupDescriptorPool()
        {
            FixedArray2<VkDescriptorPoolSize> poolSizes = new FixedArray2<VkDescriptorPoolSize>(
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 6),
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, 8));

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                Initializers.descriptorPoolCreateInfo(
                    poolSizes.Count,
                    &poolSizes.First,
                    5);

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, out descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            NativeList<VkDescriptorSetLayoutBinding> setLayoutBindings = new NativeList<VkDescriptorSetLayoutBinding>();
            VkDescriptorSetLayoutCreateInfo descriptorLayoutInfo;
            VkPipelineLayoutCreateInfo pipelineLayoutInfo;

            // Binding 0 : Vertex shader uniform buffer
            setLayoutBindings.Add(Initializers.descriptorSetLayoutBinding(
                VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                VK_SHADER_STAGE_VERTEX_BIT,
                0));
            // Binding 1 : Fragment shader image sampler
            setLayoutBindings.Add(Initializers.descriptorSetLayoutBinding(
                VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                VK_SHADER_STAGE_FRAGMENT_BIT,
                1));
            // Binding 2 : Fragment shader image sampler
            setLayoutBindings.Add(Initializers.descriptorSetLayoutBinding(
                VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                VK_SHADER_STAGE_FRAGMENT_BIT,
                2));

            // Shaded layouts (only use first layout binding)
            descriptorLayoutInfo = Initializers.descriptorSetLayoutCreateInfo((VkDescriptorSetLayoutBinding*)setLayoutBindings.Data, 1);
            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayoutInfo, null, out descriptorSetLayouts_shaded));

            var dsl = descriptorSetLayouts_shaded;
            pipelineLayoutInfo = Initializers.pipelineLayoutCreateInfo(&dsl, 1);

            Util.CheckResult(vkCreatePipelineLayout(device, &pipelineLayoutInfo, null, out pipelineLayouts_shaded));

            // Textured layouts (use all layout bindings)
            descriptorLayoutInfo = Initializers.descriptorSetLayoutCreateInfo((VkDescriptorSetLayoutBinding*)setLayoutBindings.Data, (uint)(setLayoutBindings.Count));
            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayoutInfo, null, out descriptorSetLayouts_textured));

            dsl = descriptorSetLayouts_textured;
            pipelineLayoutInfo = Initializers.pipelineLayoutCreateInfo(&dsl, 1);
            Util.CheckResult(vkCreatePipelineLayout(device, &pipelineLayoutInfo, null, out pipelineLayouts_textured));
        }

        void setupDescriptorSet()
        {
            var dsl = descriptorSetLayouts_textured;
            // Mirror plane descriptor set
            VkDescriptorSetAllocateInfo allocInfo =
                Initializers.descriptorSetAllocateInfo(
                    descriptorPool,
                    &dsl,
                    1);

            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSets_mirror));

            var descriptor0 = uniformBuffers_vsMirror.descriptor;
            var descriptor1 = offscreenPass.descriptor;
            var descriptor2 = textures_colorMap.descriptor;
            FixedArray3<VkWriteDescriptorSet> writeDescriptorSets = new FixedArray3<VkWriteDescriptorSet>(
                // Binding 0 : Vertex shader uniform buffer
                Initializers.writeDescriptorSet(
                    descriptorSets_mirror,
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    0,
                    &descriptor0),
                // Binding 1 : Fragment shader texture sampler
                Initializers.writeDescriptorSet(
                    descriptorSets_mirror,
                    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                    1,
                    &descriptor1),
                // Binding 2 : Fragment shader texture sampler
                Initializers.writeDescriptorSet(
                    descriptorSets_mirror,
                    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                    2,
                    &descriptor2));

            vkUpdateDescriptorSets(device, writeDescriptorSets.Count, &writeDescriptorSets.First, 0, null);

            // Debug quad
            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSets_debugQuad));

            var descriptor3 = uniformBuffers_vsDebugQuad.descriptor;
            var descriptor4 = offscreenPass.descriptor;
            FixedArray2<VkWriteDescriptorSet> debugQuadWriteDescriptorSets = new FixedArray2<VkWriteDescriptorSet>(
                // Binding 0 : Vertex shader uniform buffer
                Initializers.writeDescriptorSet(
                    descriptorSets_debugQuad,
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    0,
                    &descriptor3),
                // Binding 1 : Fragment shader texture sampler
                Initializers.writeDescriptorSet(
                    descriptorSets_debugQuad,
                    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                    1,
                    &descriptor4));

            vkUpdateDescriptorSets(device, debugQuadWriteDescriptorSets.Count, &debugQuadWriteDescriptorSets.First, 0, null);

            var dsls_shaded = descriptorSetLayouts_shaded;
            // Shaded descriptor sets
            allocInfo.pSetLayouts = &dsls_shaded;

            // Model
            // No texture
            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSets_model));

            var descriptor5 = uniformBuffers_vsShared.descriptor;
            VkWriteDescriptorSet modelWriteDescriptorSets =
                // Binding 0 : Vertex shader uniform buffer
                Initializers.writeDescriptorSet(
                    descriptorSets_model,
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    0,
                    &descriptor5);
            vkUpdateDescriptorSets(device, 1, &modelWriteDescriptorSets, 0, null);

            // Offscreen
            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSets_offscreen));

            var descriptor6 = uniformBuffers_vsOffScreen.descriptor;
            VkWriteDescriptorSet offScreenWriteDescriptorSets =
            // Binding 0 : Vertex shader uniform buffer
            Initializers.writeDescriptorSet(
                descriptorSets_offscreen,
                VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                0,
                &descriptor6);

            vkUpdateDescriptorSets(device, 1, &offScreenWriteDescriptorSets, 0, null);
        }

        void preparePipelines()
        {
            VkPipelineInputAssemblyStateCreateInfo inputAssemblyState =
                Initializers.pipelineInputAssemblyStateCreateInfo(
                    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                    0,
                    VK_FALSE);

            VkPipelineRasterizationStateCreateInfo rasterizationState =
                Initializers.pipelineRasterizationStateCreateInfo(
                    VK_POLYGON_MODE_FILL,
                    VK_CULL_MODE_FRONT_BIT,
                    VK_FRONT_FACE_CLOCKWISE,
                    0);

            VkPipelineColorBlendAttachmentState blendAttachmentState =
                Initializers.pipelineColorBlendAttachmentState(
                    0xf,
                    VK_FALSE);

            VkPipelineColorBlendStateCreateInfo colorBlendState =
                Initializers.pipelineColorBlendStateCreateInfo(
                    1,
                    &blendAttachmentState);

            VkPipelineDepthStencilStateCreateInfo depthStencilState =
                Initializers.pipelineDepthStencilStateCreateInfo(
                    VK_TRUE,
                    VK_TRUE,
                    VK_COMPARE_OP_LESS_OR_EQUAL);

            VkPipelineViewportStateCreateInfo viewportState =
                Initializers.pipelineViewportStateCreateInfo(1, 1, 0);

            VkPipelineMultisampleStateCreateInfo multisampleState =
                Initializers.pipelineMultisampleStateCreateInfo(
                    VkSampleCountFlags.Count1,
                    0);

            FixedArray2<VkDynamicState> dynamicStateEnables = new FixedArray2<VkDynamicState>(
                VK_DYNAMIC_STATE_VIEWPORT,
                VK_DYNAMIC_STATE_SCISSOR);

            VkPipelineDynamicStateCreateInfo dynamicState =
                Initializers.pipelineDynamicStateCreateInfo(
                    &dynamicStateEnables.First,
                    dynamicStateEnables.Count,
                    0);

            // Solid rendering pipeline
            // Load shaders
            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStages = new FixedArray2<VkPipelineShaderStageCreateInfo>(
                loadShader(getAssetPath() + "shaders/offscreen/quad.vert.spv", VK_SHADER_STAGE_VERTEX_BIT),
                loadShader(getAssetPath() + "shaders/offscreen/quad.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT));

            VkGraphicsPipelineCreateInfo pipelineCreateInfo =
                Initializers.pipelineCreateInfo(
                    pipelineLayouts_textured,
                    renderPass,
                    0);

            var vis = vertices_inputState;
            pipelineCreateInfo.pVertexInputState = &vis;
            pipelineCreateInfo.pInputAssemblyState = &inputAssemblyState;
            pipelineCreateInfo.pRasterizationState = &rasterizationState;
            pipelineCreateInfo.pColorBlendState = &colorBlendState;
            pipelineCreateInfo.pMultisampleState = &multisampleState;
            pipelineCreateInfo.pViewportState = &viewportState;
            pipelineCreateInfo.pDepthStencilState = &depthStencilState;
            pipelineCreateInfo.pDynamicState = &dynamicState;
            pipelineCreateInfo.stageCount = shaderStages.Count;
            pipelineCreateInfo.pStages = &shaderStages.First;

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_debug));

            // Mirror
            shaderStages.First = loadShader(getAssetPath() + "shaders/offscreen/mirror.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/offscreen/mirror.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);
            rasterizationState.cullMode = VK_CULL_MODE_NONE;
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_mirror));

            // Flip culling
            rasterizationState.cullMode = VK_CULL_MODE_BACK_BIT;

            // Phong shading pipelines
            pipelineCreateInfo.layout = pipelineLayouts_shaded;
            // Scene
            shaderStages.First = loadShader(getAssetPath() + "shaders/offscreen/phong.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/offscreen/phong.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_shaded));
            // Offscreen
            // Flip culling
            rasterizationState.cullMode = VK_CULL_MODE_FRONT_BIT;
            pipelineCreateInfo.renderPass = offscreenPass.renderPass;
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_shadedOffscreen));

        }

        // Prepare and initialize uniform buffer containing shader uniforms
        void prepareUniformBuffers()
        {
            // Mesh vertex shader uniform buffer block
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffers_vsShared,
                (ulong)sizeof(UBO)));

            // Mirror plane vertex shader uniform buffer block
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffers_vsMirror,
                (ulong)sizeof(UBO)));

            // Offscreen vertex shader uniform buffer block 
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffers_vsOffScreen,
                (ulong)sizeof(UBO)));

            // Debug quad vertex shader uniform buffer block 
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffers_vsDebugQuad,
                (ulong)sizeof(UBO)));

            // Map persistent
            Util.CheckResult(uniformBuffers_vsShared.map());
            Util.CheckResult(uniformBuffers_vsMirror.map());
            Util.CheckResult(uniformBuffers_vsOffScreen.map());
            Util.CheckResult(uniformBuffers_vsDebugQuad.map());

            updateUniformBuffers();
            updateUniformBufferOffscreen();
        }

        void updateUniformBuffers()
        {
            // Mesh
            uboShared.projection = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(60.0f), (float)width / (float)height, 0.1f, 256.0f);
            Matrix4x4 viewMatrix = Matrix4x4.CreateTranslation(new Vector3(0, 0, zoom));

            uboShared.model = viewMatrix * Matrix4x4.CreateTranslation(cameraPos);
            uboShared.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * uboShared.model;
            uboShared.model = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y + meshRot.Y)) * uboShared.model;
            uboShared.model = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * uboShared.model;

            uboShared.model = Matrix4x4.CreateTranslation(meshPos) * uboShared.model;
            // uboShared.model = glm::translate(uboShared.model, meshPos);

            var local = uboShared;
            Unsafe.CopyBlock(uniformBuffers_vsShared.mapped, &local, (uint)sizeof(UBO));

            // Mirror
            uboShared.model = viewMatrix * Matrix4x4.CreateTranslation(cameraPos);
            uboShared.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * uboShared.model;
            uboShared.model = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y)) * uboShared.model;
            uboShared.model = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * uboShared.model;

            var local2 = uboShared;
            Unsafe.CopyBlock(uniformBuffers_vsMirror.mapped, &local2, (uint)sizeof(UBO));

            // Debug quad
            uboShared.projection = Matrix4x4.CreateOrthographicOffCenter(4f, 0f, 0f, 4f * (float)height / (float)width, -1f, 1f);
            uboShared.model = Matrix4x4.Identity;

            var local3 = uboShared;
            Unsafe.CopyBlock(uniformBuffers_vsDebugQuad.mapped, &local3, (uint)sizeof(UBO));
        }

        void updateUniformBufferOffscreen()
        {
            uboShared.projection = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(60f), (float)width / (float)height, 0.1f, 256.0f);
            Matrix4x4 viewMatrix = Matrix4x4.CreateTranslation(new Vector3(0.0f, 0.0f, zoom));

            uboShared.model = viewMatrix * Matrix4x4.CreateTranslation(cameraPos);
            uboShared.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * uboShared.model;
            uboShared.model = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y + meshRot.Y)) * uboShared.model;
            uboShared.model = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * uboShared.model;

            uboShared.model = Matrix4x4.CreateScale(new Vector3(1, -1, 1)) * uboShared.model;
            uboShared.model = Matrix4x4.CreateTranslation(meshPos) * uboShared.model;

            var local = uboShared;
            Unsafe.CopyBlock(uniformBuffers_vsOffScreen.mapped, &local, (uint)sizeof(UBO));
        }

        void draw()
        {
            prepareFrame();

            // The scene render command buffer has to wait for the offscreen
            // rendering to be finished before we can use the framebuffer 
            // color image for sampling during final rendering
            // To ensure this we use a dedicated offscreen synchronization
            // semaphore that will be signaled when offscreen renderin
            // has been finished
            // This is necessary as an implementation may start both
            // command buffers at the same time, there is no guarantee
            // that command buffers will be executed in the order they
            // have been submitted by the application

            // Offscreen rendering

            // Wait for swap chain presentation to finish
            submitInfo.pWaitSemaphores = (VkSemaphore*)Unsafe.AsPointer(ref semaphores[0].PresentComplete);
            // Signal ready with offscreen semaphore
            var offscreenSemaphore = offscreenPass.semaphore;
            submitInfo.pSignalSemaphores = &offscreenSemaphore;

            // Submit work
            var offscreenCommandBuffer = offscreenPass.commandBuffer;
            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = &offscreenCommandBuffer;
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, VkFence.Null));

            // Scene rendering

            // Wait for offscreen semaphore
            submitInfo.pWaitSemaphores = (VkSemaphore*)Unsafe.AsPointer(ref offscreenPass.semaphore);
            // Signal ready with render complete semaphpre
            submitInfo.pSignalSemaphores = (VkSemaphore*)Unsafe.AsPointer(ref semaphores[0].RenderComplete);

            // Submit work
            submitInfo.pCommandBuffers = (VkCommandBuffer*)drawCmdBuffers.GetAddress(currentBuffer);
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, VkFence.Null));

            submitFrame();
        }
        public override void Prepare()
        {
            base.Prepare();
            loadAssets();
            generateQuad();
            prepareOffscreen();
            setupVertexDescriptions();
            prepareUniformBuffers();
            setupDescriptorSetLayout();
            preparePipelines();
            setupDescriptorPool();
            setupDescriptorSet();
            buildCommandBuffers();
            buildOffscreenCommandBuffer();
            prepared = true;
        }

        protected override void render()
        {
            if (!prepared)
                return;
            draw();
            if (!paused)
            {
                meshRot.Y += frameTimer * 10.0f;
                updateUniformBuffers();
                updateUniformBufferOffscreen();
            }
        }

        protected override void viewChanged()
        {
            updateUniformBuffers();
            updateUniformBufferOffscreen();
        }

        protected override void keyPressed(Key key)
        {
            switch (key)
            {
                case Key.D:
                    toggleDebugDisplay();
                    break;
            }
        }

        void toggleDebugDisplay()
        {
            debugDisplay = !debugDisplay;
            reBuildCommandBuffers();
        }

        public static void Main() => new OffscreenExample().ExampleMain();
    }
}
