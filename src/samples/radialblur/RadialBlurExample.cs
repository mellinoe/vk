// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: radialblur/radialblur.cpp

/*
* Vulkan Example - Fullscreen radial blur (Single pass offscreen effect)
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using Vulkan;
using static Vulkan.VulkanNative;
using static Vulkan.RawConstants;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Veldrid.Sdl2;
using Veldrid;

namespace Vk.Samples
{
    public unsafe class RadialBlurExample : VulkanExampleBase
    {

        private const uint VERTEX_BUFFER_BIND_ID = 0;
        private const int FB_DIM = 512;
        private const VkFormat FB_COLOR_FORMAT = VK_FORMAT_R8G8B8A8_UNORM;

        bool blur = true;
        bool displayTexture = false;

        vksTexture2D textures_gradient = new vksTexture2D();

        // Vertex layout for the models
        vksVertexLayout vertexLayout = new vksVertexLayout(
            VertexComponent.VERTEX_COMPONENT_POSITION,
            VertexComponent.VERTEX_COMPONENT_UV,
            VertexComponent.VERTEX_COMPONENT_COLOR,
            VertexComponent.VERTEX_COMPONENT_NORMAL);

        vksModel models_example = new vksModel();

        VkPipelineVertexInputStateCreateInfo vertices_inputState;
        NativeList<VkVertexInputBindingDescription> vertices_bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
        NativeList<VkVertexInputAttributeDescription> vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();

        vksBuffer uniformBuffers_scene = new vksBuffer();
        vksBuffer uniformBuffers_blurParams = new vksBuffer();

        struct UboVS
        {
            public Matrix4x4 projection;
            public Matrix4x4 model;
            public float gradientPos;
        }

        UboVS uboScene;

        struct UboBlurParams
        {
            public float radialBlurScale;
            public float radialBlurStrength;
            public Vector2 radialOrigin;
        }

        UboBlurParams uboBlurParams = new UboBlurParams()
        {
            radialBlurScale = 0.35f,
            radialBlurStrength = 0.75f,
            radialOrigin = new Vector2(0.5f, 0.5f)
        };

        VkPipeline pipelines_radialBlur;
        VkPipeline pipelines_colorPass;
        VkPipeline pipelines_phongPass;
        VkPipeline pipelines_offscreenDisplay;

        VkPipelineLayout pipelineLayouts_radialBlur;
        VkPipelineLayout pipelineLayouts_scene;

        VkDescriptorSet descriptorSets_scene;
        VkDescriptorSet descriptorSets_radialBlur;

        VkDescriptorSetLayout descriptorSetLayouts_scene;
        VkDescriptorSetLayout descriptorSetLayouts_radialBlur;

        // Framebuffer for offscreen rendering
        struct FrameBufferAttachment
        {
            public VkImage image;
            public VkDeviceMemory mem;
            public VkImageView view;
        }

        struct OffscreenPass
        {
            public uint width, height;
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

        public RadialBlurExample()
        {
            zoom = -10.0f;
            rotation = new Vector3(-16.25f, -28.75f, 0.0f);
            timerSpeed *= 0.5f;
            enableTextOverlay = true;
            title = "Vulkan Example - Radial blur";
        }

        public void Dispose()
        {
            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class

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

            vkDestroyPipeline(device, pipelines_radialBlur, null);
            vkDestroyPipeline(device, pipelines_phongPass, null);
            vkDestroyPipeline(device, pipelines_colorPass, null);
            vkDestroyPipeline(device, pipelines_offscreenDisplay, null);

            vkDestroyPipelineLayout(device, pipelineLayouts_radialBlur, null);
            vkDestroyPipelineLayout(device, pipelineLayouts_scene, null);

            vkDestroyDescriptorSetLayout(device, descriptorSetLayouts_scene, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayouts_radialBlur, null);

            models_example.destroy();

            uniformBuffers_scene.destroy();
            uniformBuffers_blurParams.destroy();

            vkFreeCommandBuffers(device, cmdPool, 1, ref offscreenPass.commandBuffer);
            vkDestroySemaphore(device, offscreenPass.semaphore, null);

            textures_gradient.destroy();
        }

        // Setup the offscreen framebuffer for rendering the blurred scene
        // The color attachment of this framebuffer will then be used to sample frame in the fragment shader of the final pass
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
            image.samples = VK_SAMPLE_COUNT_1_BIT;
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
            colorImageView.subresourceRange = new VkImageSubresourceRange();
            colorImageView.subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
            colorImageView.subresourceRange.baseMipLevel = 0;
            colorImageView.subresourceRange.levelCount = 1;
            colorImageView.subresourceRange.baseArrayLayer = 0;
            colorImageView.subresourceRange.layerCount = 1;
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
            attchmentDescriptions.First.samples = VK_SAMPLE_COUNT_1_BIT;
            attchmentDescriptions.First.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;
            attchmentDescriptions.First.storeOp = VK_ATTACHMENT_STORE_OP_STORE;
            attchmentDescriptions.First.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
            attchmentDescriptions.First.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
            attchmentDescriptions.First.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
            attchmentDescriptions.First.finalLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            // Depth attachment
            attchmentDescriptions.Second.format = fbDepthFormat;
            attchmentDescriptions.Second.samples = VK_SAMPLE_COUNT_1_BIT;
            attchmentDescriptions.Second.loadOp = VK_ATTACHMENT_LOAD_OP_CLEAR;
            attchmentDescriptions.Second.storeOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
            attchmentDescriptions.Second.stencilLoadOp = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
            attchmentDescriptions.Second.stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
            attchmentDescriptions.Second.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
            attchmentDescriptions.Second.finalLayout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL;

            VkAttachmentReference colorReference = new VkAttachmentReference { attachment = 0, layout = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL };
            VkAttachmentReference depthReference = new VkAttachmentReference { attachment = 1, layout = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL };

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

            FixedArray2<VkImageView> attachments = new FixedArray2<VkImageView>(offscreenPass.color.view, offscreenPass.depth.view);

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
                VkSemaphoreCreateInfo semaphoreCreateInfo = Initializers.semaphoreCreateInfo();
                Util.CheckResult(vkCreateSemaphore(device, &semaphoreCreateInfo, null, out offscreenPass.semaphore));
            }

            VkCommandBufferBeginInfo cmdBufInfo = Initializers.commandBufferBeginInfo();

            FixedArray2<VkClearValue> clearValues = new FixedArray2<VkClearValue>();
            clearValues.First.color = new VkClearColorValue(0.0f, 0.0f, 0.0f, 0.0f);
            clearValues.Second.depthStencil = new VkClearDepthStencilValue { depth = 1.0f, stencil = 0 };

            VkRenderPassBeginInfo renderPassBeginInfo = Initializers.renderPassBeginInfo();
            renderPassBeginInfo.renderPass = offscreenPass.renderPass;
            renderPassBeginInfo.framebuffer = offscreenPass.frameBuffer;
            renderPassBeginInfo.renderArea.extent.width = offscreenPass.width;
            renderPassBeginInfo.renderArea.extent.height = offscreenPass.height;
            renderPassBeginInfo.clearValueCount = 2;
            renderPassBeginInfo.pClearValues = &clearValues.First;

            Util.CheckResult(vkBeginCommandBuffer(offscreenPass.commandBuffer, &cmdBufInfo));

            VkViewport viewport = Initializers.viewport((float)offscreenPass.width, (float)offscreenPass.height, 0.0f, 1.0f);
            vkCmdSetViewport(offscreenPass.commandBuffer, 0, 1, &viewport);

            VkRect2D scissor = Initializers.rect2D(offscreenPass.width, offscreenPass.height, 0, 0);
            vkCmdSetScissor(offscreenPass.commandBuffer, 0, 1, &scissor);

            vkCmdBeginRenderPass(offscreenPass.commandBuffer, &renderPassBeginInfo, VK_SUBPASS_CONTENTS_INLINE);

            vkCmdBindDescriptorSets(offscreenPass.commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayouts_scene, 0, 1, ref descriptorSets_scene, 0, null);
            vkCmdBindPipeline(offscreenPass.commandBuffer, VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_colorPass);

            ulong offsets = 0;
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

                // 3D scene
                vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayouts_scene, 0, 1, ref descriptorSets_scene, 0, null);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_phongPass);

                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_example.vertices.buffer, &offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], models_example.indices.buffer, 0, VK_INDEX_TYPE_UINT32);
                vkCmdDrawIndexed(drawCmdBuffers[i], models_example.indexCount, 1, 0, 0, 0);

                // Fullscreen triangle (clipped to a quad) with radial blur
                if (blur)
                {
                    vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayouts_radialBlur, 0, 1, ref descriptorSets_radialBlur, 0, null);
                    vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, (displayTexture) ? pipelines_offscreenDisplay : pipelines_radialBlur);
                    vkCmdDraw(drawCmdBuffers[i], 3, 1, 0, 0);
                }

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void loadAssets()
        {
            models_example.loadFromFile(getAssetPath() + "models/glowsphere.dae", vertexLayout, 0.05f, vulkanDevice, queue);
            textures_gradient.loadFromFile(getAssetPath() + "textures/particle_gradient_rgba.ktx", VK_FORMAT_R8G8B8A8_UNORM, vulkanDevice, queue);
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
            // Example uses three ubos and one image sampler
            NativeList<VkDescriptorPoolSize> poolSizes = new NativeList<VkDescriptorPoolSize>
            {
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 4),
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, 6)
            };

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                Initializers.descriptorPoolCreateInfo(
                    poolSizes.Count,
                    (VkDescriptorPoolSize*)poolSizes.Data,
                    2);

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, out descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            NativeList<VkDescriptorSetLayoutBinding> setLayoutBindings;
            VkDescriptorSetLayoutCreateInfo descriptorLayout;
            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo;

            // Scene rendering
            setLayoutBindings = new NativeList<VkDescriptorSetLayoutBinding>
            {
                // Binding 0: Vertex shader uniform buffer
                Initializers.descriptorSetLayoutBinding(
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    VK_SHADER_STAGE_VERTEX_BIT,
                    0),
                // Binding 1: Fragment shader image sampler
                Initializers.descriptorSetLayoutBinding(
                    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                    VK_SHADER_STAGE_FRAGMENT_BIT,
                    1),
                // Binding 2: Fragment shader uniform buffer
                Initializers.descriptorSetLayoutBinding(
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    VK_SHADER_STAGE_FRAGMENT_BIT,
                    2)
            };

            descriptorLayout = Initializers.descriptorSetLayoutCreateInfo((VkDescriptorSetLayoutBinding*)setLayoutBindings.Data, setLayoutBindings.Count);
            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayout, null, out descriptorSetLayouts_scene));
            var dsl = descriptorSetLayouts_scene;
            pPipelineLayoutCreateInfo = Initializers.pipelineLayoutCreateInfo(&dsl, 1);
            Util.CheckResult(vkCreatePipelineLayout(device, &pPipelineLayoutCreateInfo, null, out pipelineLayouts_scene));

            // Fullscreen radial blur
            setLayoutBindings = new NativeList<VkDescriptorSetLayoutBinding>
            {
                // Binding 0 : Vertex shader uniform buffer
                Initializers.descriptorSetLayoutBinding(
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    VK_SHADER_STAGE_FRAGMENT_BIT,
                    0),
                // Binding 0: Fragment shader image sampler
                Initializers.descriptorSetLayoutBinding(
                    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                    VK_SHADER_STAGE_FRAGMENT_BIT,
                    1)
            };
            descriptorLayout = Initializers.descriptorSetLayoutCreateInfo((VkDescriptorSetLayoutBinding*)setLayoutBindings.Data, setLayoutBindings.Count);
            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayout, null, out descriptorSetLayouts_radialBlur));
            dsl = descriptorSetLayouts_radialBlur;
            pPipelineLayoutCreateInfo = Initializers.pipelineLayoutCreateInfo(&dsl, 1);
            Util.CheckResult(vkCreatePipelineLayout(device, &pPipelineLayoutCreateInfo, null, out pipelineLayouts_radialBlur));
        }

        void setupDescriptorSet()
        {
            VkDescriptorSetAllocateInfo descriptorSetAllocInfo;

            // Scene rendering
            var dsl = descriptorSetLayouts_scene;
            descriptorSetAllocInfo = Initializers.descriptorSetAllocateInfo(descriptorPool, &dsl, 1);
            Util.CheckResult(vkAllocateDescriptorSets(device, &descriptorSetAllocInfo, out descriptorSets_scene));

            var descriptor0 = uniformBuffers_scene.descriptor;
            var descriptor1 = textures_gradient.descriptor;
            NativeList<VkWriteDescriptorSet> offScreenWriteDescriptorSets = new NativeList<VkWriteDescriptorSet>
            {
                // Binding 0: Vertex shader uniform buffer
                Initializers.writeDescriptorSet(
                    descriptorSets_scene,
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    0,
                    &descriptor0),
                // Binding 1: Color gradient sampler
                Initializers.writeDescriptorSet(
                    descriptorSets_scene,
                    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                    1,
                    &descriptor1),
            };
            vkUpdateDescriptorSets(device, offScreenWriteDescriptorSets.Count, offScreenWriteDescriptorSets.Data, 0, null);

            // Fullscreen radial blur
            dsl = descriptorSetLayouts_radialBlur;
            descriptorSetAllocInfo = Initializers.descriptorSetAllocateInfo(descriptorPool, &dsl, 1);
            Util.CheckResult(vkAllocateDescriptorSets(device, &descriptorSetAllocInfo, out descriptorSets_radialBlur));

            descriptor0 = uniformBuffers_blurParams.descriptor;
            descriptor1 = offscreenPass.descriptor;
            NativeList<VkWriteDescriptorSet> writeDescriptorSets = new NativeList<VkWriteDescriptorSet>
            {
                // Binding 0: Vertex shader uniform buffer
                Initializers.writeDescriptorSet(
                    descriptorSets_radialBlur,
                    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                    0,
                    &descriptor0),
                // Binding 0: Fragment shader texture sampler
                Initializers.writeDescriptorSet(
                    descriptorSets_radialBlur,
                    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                    1,
                    &descriptor1),
            };

            vkUpdateDescriptorSets(device, writeDescriptorSets.Count, writeDescriptorSets.Data, 0, null);
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
                    VK_CULL_MODE_NONE,
                    VK_FRONT_FACE_COUNTER_CLOCKWISE,
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
                    VK_SAMPLE_COUNT_1_BIT,
                    0);

            NativeList<VkDynamicState> dynamicStateEnables = new NativeList<VkDynamicState>
            {
                VK_DYNAMIC_STATE_VIEWPORT,
                VK_DYNAMIC_STATE_SCISSOR
            };
            VkPipelineDynamicStateCreateInfo dynamicState =
                Initializers.pipelineDynamicStateCreateInfo(
                    (VkDynamicState*)dynamicStateEnables.Data,
                    dynamicStateEnables.Count,
                    0);

            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStages = new FixedArray2<VkPipelineShaderStageCreateInfo>();

            VkGraphicsPipelineCreateInfo pipelineCreateInfo =
                Initializers.pipelineCreateInfo(
                    pipelineLayouts_radialBlur,
                    renderPass,
                    0);

            pipelineCreateInfo.pInputAssemblyState = &inputAssemblyState;
            pipelineCreateInfo.pRasterizationState = &rasterizationState;
            pipelineCreateInfo.pColorBlendState = &colorBlendState;
            pipelineCreateInfo.pMultisampleState = &multisampleState;
            pipelineCreateInfo.pViewportState = &viewportState;
            pipelineCreateInfo.pDepthStencilState = &depthStencilState;
            pipelineCreateInfo.pDynamicState = &dynamicState;
            pipelineCreateInfo.stageCount = shaderStages.Count;
            pipelineCreateInfo.pStages = &shaderStages.First;

            // Radial blur pipeline
            shaderStages.First = loadShader(getAssetPath() + "shaders/radialblur/radialblur.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/radialblur/radialblur.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);
            // Empty vertex input state
            VkPipelineVertexInputStateCreateInfo emptyInputState = Initializers.pipelineVertexInputStateCreateInfo();
            pipelineCreateInfo.pVertexInputState = &emptyInputState;
            pipelineCreateInfo.layout = pipelineLayouts_radialBlur;
            // Additive blending
            blendAttachmentState.colorWriteMask = (VkColorComponentFlags)0xF;
            blendAttachmentState.blendEnable = VK_TRUE;
            blendAttachmentState.colorBlendOp = VK_BLEND_OP_ADD;
            blendAttachmentState.srcColorBlendFactor = VK_BLEND_FACTOR_ONE;
            blendAttachmentState.dstColorBlendFactor = VK_BLEND_FACTOR_ONE;
            blendAttachmentState.alphaBlendOp = VK_BLEND_OP_ADD;
            blendAttachmentState.srcAlphaBlendFactor = VK_BLEND_FACTOR_SRC_ALPHA;
            blendAttachmentState.dstAlphaBlendFactor = VK_BLEND_FACTOR_DST_ALPHA;
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_radialBlur));

            // No blending (for debug display)
            blendAttachmentState.blendEnable = VK_FALSE;
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_offscreenDisplay));

            // Phong pass
            pipelineCreateInfo.layout = pipelineLayouts_scene;
            shaderStages.First = loadShader(getAssetPath() + "shaders/radialblur/phongpass.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/radialblur/phongpass.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);
            var vis = vertices_inputState;
            pipelineCreateInfo.pVertexInputState = &vis;
            blendAttachmentState.blendEnable = VK_FALSE;
            depthStencilState.depthWriteEnable = VK_TRUE;
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_phongPass));

            // Color only pass (offscreen blur base)
            shaderStages.First = loadShader(getAssetPath() + "shaders/radialblur/colorpass.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/radialblur/colorpass.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);
            pipelineCreateInfo.renderPass = offscreenPass.renderPass;
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_colorPass));
        }

        // Prepare and initialize uniform buffer containing shader uniforms
        void prepareUniformBuffers()
        {
            // Phong and color pass vertex shader uniform buffer
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffers_scene,
                (ulong)sizeof(UboVS)));

            // Fullscreen radial blur parameters
            var blurParamsLocal = uboBlurParams;
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffers_blurParams,
                (ulong)sizeof(UboBlurParams),
                &blurParamsLocal));

            // Map persistent
            Util.CheckResult(uniformBuffers_scene.map());
            Util.CheckResult(uniformBuffers_blurParams.map());

            updateUniformBuffersScene();
        }

        // Update uniform buffers for rendering the 3D scene
        void updateUniformBuffersScene()
        {
            uboScene.projection = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(45.0f), (float)width / (float)height, 1.0f, 256.0f);
            Matrix4x4 viewMatrix = Matrix4x4.CreateTranslation(0, 0, zoom);

            uboScene.model = Matrix4x4.Identity;
            uboScene.model = viewMatrix * uboScene.model * Matrix4x4.CreateTranslation(cameraPos);
            uboScene.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * uboScene.model;
            uboScene.model = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y)) * uboScene.model;
            uboScene.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(timer * 360.0f)) * uboScene.model;
            uboScene.model = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * uboScene.model;

            if (!paused)
            {
                uboScene.gradientPos += frameTimer * 0.1f;
            }

            var local = uboScene;
            Unsafe.CopyBlock(uniformBuffers_scene.mapped, &local, (uint)sizeof(UboVS));
        }

        void draw()
        {
            prepareFrame();

            // Offscreen rendering

            // Wait for swap chain presentation to finish
            submitInfo.pWaitSemaphores = (VkSemaphore*)Unsafe.AsPointer(ref semaphores[0].PresentComplete);
            // Signal ready with offscreen semaphore
            var signalSemaphore = offscreenPass.semaphore;
            submitInfo.pSignalSemaphores = &signalSemaphore;

            // Submit work
            submitInfo.commandBufferCount = 1;
            var commandBuffer = offscreenPass.commandBuffer;
            submitInfo.pCommandBuffers = &commandBuffer;
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, VkFence.Null));

            // Scene rendering

            // Wait for offscreen semaphore
            var semaphore = offscreenPass.semaphore;
            submitInfo.pWaitSemaphores = &semaphore;
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
                updateUniformBuffersScene();
            }
        }

        protected override void viewChanged()
        {
            updateUniformBuffersScene();
        }

        void toggleBlur()
        {
            blur = !blur;
            updateUniformBuffersScene();
            reBuildCommandBuffers();
        }

        void toggleTextureDisplay()
        {
            displayTexture = !displayTexture;
            reBuildCommandBuffers();
        }

        protected override void keyPressed(Key key)
        {
            switch (key)
            {
                case Key.B:
                    toggleBlur();
                    break;
                case Key.T:
                    toggleTextureDisplay();
                    break;
            }
        }

        public static void Main() => new RadialBlurExample().ExampleMain();
    }
}
