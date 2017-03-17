using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class TextureMappingExample : VulkanExampleBase, IDisposable
    {
        // Contains all Vulkan objects that are required to store and use a texture
        // Note that this repository contains a texture class (VulkanTexture.hpp) that encapsulates texture loading functionality in a class that is used in subsequent demos
        public struct Texture
        {
            public VkSampler sampler;
            public VkImage image;
            public VkImageLayout imageLayout;
            public VkDeviceMemory DeviceMemory;
            public VkImageView view;
            public uint width, height;
            public uint mipLevels;
        }

        Texture texture;

        public class Vertices
        {
            public VkPipelineVertexInputStateCreateInfo inputState;
            public NativeList<VkVertexInputBindingDescription> bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
            public NativeList<VkVertexInputAttributeDescription> attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();
        }

        Vertices vertices = new Vertices();

        vksBuffer vertexBuffer = new Samples.vksBuffer();
        vksBuffer indexBuffer = new vksBuffer();
        uint indexCount;

        vksBuffer uniformBufferVS = new vksBuffer();

        public struct UboVS
        {
            public Matrix4x4 projection;
            public Matrix4x4 model;
            public Vector4 viewPos;
            public float lodBias;
        }

        UboVS uboVS;
        VkPipeline pipelines_solid;

        VkPipelineLayout pipelineLayout;
        VkDescriptorSet descriptorSet;
        VkDescriptorSetLayout descriptorSetLayout;


        TextureMappingExample()
        {
            zoom = -2.5f;
            rotation = new Vector3(0.0f, 15.0f, 0.0f);
            Title = "Vulkan Example - Texture loading";
            // enableTextOverlay = true;
        }

        public void Dispose()
        {
            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class

            destroyTextureImage(texture);

            vkDestroyPipeline(Device, pipelines_solid, null);

            vkDestroyPipelineLayout(Device, pipelineLayout, null);
            vkDestroyDescriptorSetLayout(Device, descriptorSetLayout, null);

            vertexBuffer.destroy();
            indexBuffer.destroy();
            uniformBufferVS.destroy();
        }

        // Create an image memory barrier for changing the layout of
        // an image and put it into an active command buffer
        void setImageLayout(
            VkCommandBuffer cmdBuffer,
            VkImage image,
            VkImageAspectFlags aspectMask,
            VkImageLayout oldImageLayout,
            VkImageLayout newImageLayout,
            VkImageSubresourceRange subresourceRange)
        {
            // Create an image barrier object
            VkImageMemoryBarrier imageMemoryBarrier = Initializers.imageMemoryBarrier(); ;
            imageMemoryBarrier.oldLayout = oldImageLayout;
            imageMemoryBarrier.newLayout = newImageLayout;
            imageMemoryBarrier.image = image;
            imageMemoryBarrier.subresourceRange = subresourceRange;

            // Only sets masks for layouts used in this example
            // For a more complete version that can be used with other layouts see vks::tools::setImageLayout

            // Source layouts (old)
            switch (oldImageLayout)
            {
                case VkImageLayout.Undefined:
                    // Only valid as initial layout, memory contents are not preserved
                    // Can be accessed directly, no source dependency required
                    imageMemoryBarrier.srcAccessMask = 0;
                    break;
                case VkImageLayout.Preinitialized:
                    // Only valid as initial layout for linear images, preserves memory contents
                    // Make sure host writes to the image have been finished
                    imageMemoryBarrier.srcAccessMask = VkAccessFlags.HostWrite;
                    break;
                case VkImageLayout.TransferDstOptimal:
                    // Old layout is transfer destination
                    // Make sure any writes to the image have been finished
                    imageMemoryBarrier.srcAccessMask = VkAccessFlags.TransferWrite;
                    break;
            }

            // Target layouts (new)
            switch (newImageLayout)
            {
                case VkImageLayout.TransferSrcOptimal:
                    // Transfer source (copy, blit)
                    // Make sure any reads from the image have been finished
                    imageMemoryBarrier.dstAccessMask = VkAccessFlags.TransferRead;
                    break;
                case VkImageLayout.TransferDstOptimal:
                    // Transfer destination (copy, blit)
                    // Make sure any writes to the image have been finished
                    imageMemoryBarrier.dstAccessMask = VkAccessFlags.TransferWrite;
                    break;
                case VkImageLayout.ShaderReadOnlyOptimal:
                    // Shader read (sampler, input attachment)
                    imageMemoryBarrier.dstAccessMask = VkAccessFlags.ShaderRead;
                    break;
            }

            // Put barrier on top of pipeline
            VkPipelineStageFlags srcStageFlags = VkPipelineStageFlags.TopOfPipe;
            VkPipelineStageFlags destStageFlags = VkPipelineStageFlags.TopOfPipe;

            // Put barrier inside setup command buffer
            vkCmdPipelineBarrier(
                cmdBuffer,
                srcStageFlags,
                destStageFlags,
                VkDependencyFlags.None,
                0, null,
                0, null,
                1, &imageMemoryBarrier);
        }

        void loadTexture(string fileName, VkFormat format, bool forceLinearTiling)
        {
            gli::texture2d tex2D(gli::load(fileName));

            assert(!tex2D.empty());

            VkFormatProperties formatProperties;

            texture.width = static_cast<uint>(tex2D[0].extent().x);
            texture.height = static_cast<uint>(tex2D[0].extent().y);
            texture.mipLevels = static_cast<uint>(tex2D.levels());

            // Get Device properites for the requested texture format
            vkGetPhysicalDeviceFormatProperties(PhysicalDevice, format, &formatProperties);

            // Only use linear tiling if requested (and supported by the Device)
            // Support for linear tiling is mostly limited, so prefer to use
            // optimal tiling instead
            // On most implementations linear tiling will only support a very
            // limited amount of formats and features (mip maps, cubemaps, arrays, etc.)
            uint useStaging = 1;

            // Only use linear tiling if forced
            if (forceLinearTiling)
            {
                // Don't use linear if format is not supported for (linear) shader sampling
                useStaging = ((formatProperties.linearTilingFeatures & VkFormatFeatureFlags.SampledImage) != VkFormatFeatureFlags.SampledImage) ? 1u : 0u;
            }

            VkMemoryAllocateInfo memAllocInfo = Initializers.memoryAllocateInfo();
            VkMemoryRequirements memReqs = new VkMemoryRequirements();

            if (useStaging == 1)
            {
                // Create a host-visible staging buffer that contains the raw image data
                VkBuffer stagingBuffer;
                VkDeviceMemory stagingMemory;

                VkBufferCreateInfo bufferCreateInfo = Initializers.bufferCreateInfo();
                bufferCreateInfo.size = tex2D.size();
                // This buffer is used as a transfer source for the buffer copy
                bufferCreateInfo.usage =  VkBufferUsageFlags.TransferSrc;
                bufferCreateInfo.sharingMode =  VkSharingMode.Exclusive;

                Util.CheckResult(vkCreateBuffer(Device, &bufferCreateInfo, null, &stagingBuffer));

                // Get memory requirements for the staging buffer (alignment, memory type bits)
                vkGetBufferMemoryRequirements(Device, stagingBuffer, &memReqs);

                memAllocInfo.allocationSize = memReqs.size;
                // Get memory type index for a host visible buffer
                memAllocInfo.memoryTypeIndex = VulkanDevice.GetMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

                Util.CheckResult(vkAllocateMemory(Device, &memAllocInfo, null, &stagingMemory));
                Util.CheckResult(vkBindBufferMemory(Device, stagingBuffer, stagingMemory, 0));

                // Copy texture data into staging buffer
                byte* data;
                Util.CheckResult(vkMapMemory(Device, stagingMemory, 0, memReqs.size, 0, (void**)&data));
                Unsafe.CopyBlock(data, tex2D.data(), tex2D.size());
                vkUnmapMemory(Device, stagingMemory);

                // Setup buffer copy regions for each mip level
                std::vector<VkBufferImageCopy> bufferCopyRegions;
                uint offset = 0;

                for (uint i = 0; i < texture.mipLevels; i++)
                {
                    VkBufferImageCopy bufferCopyRegion = new VkBufferImageCopy();
                    bufferCopyRegion.imageSubresource.aspectMask =  VkImageAspectFlags.Color;
                    bufferCopyRegion.imageSubresource.mipLevel = i;
                    bufferCopyRegion.imageSubresource.baseArrayLayer = 0;
                    bufferCopyRegion.imageSubresource.layerCount = 1;
                    bufferCopyRegion.imageExtent.width = static_cast<uint>(tex2D[i].extent().x);
                    bufferCopyRegion.imageExtent.height = static_cast<uint>(tex2D[i].extent().y);
                    bufferCopyRegion.imageExtent.depth = 1;
                    bufferCopyRegion.bufferOffset = offset;

                    bufferCopyRegions.push_back(bufferCopyRegion);

                    offset += static_cast<uint>(tex2D[i].size());
                }

                // Create optimal tiled target image
                VkImageCreateInfo imageCreateInfo = Initializers.imageCreateInfo();
                imageCreateInfo.imageType = VK_IMAGE_TYPE_2D;
                imageCreateInfo.format = format;
                imageCreateInfo.mipLevels = texture.mipLevels;
                imageCreateInfo.arrayLayers = 1;
                imageCreateInfo.samples = VK_SAMPLE_COUNT_1_BIT;
                imageCreateInfo.tiling = VK_IMAGE_TILING_OPTIMAL;
                imageCreateInfo.sharingMode = VK_SHARING_MODE_EXCLUSIVE;
                // Set initial layout of the image to undefined
                imageCreateInfo.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
                imageCreateInfo.extent = { texture.width, texture.height, 1 };
                imageCreateInfo.usage = VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT;

                Util.CheckResult(vkCreateImage(Device, &imageCreateInfo, null, &texture.image));

                vkGetImageMemoryRequirements(Device, texture.image, &memReqs);

                memAllocInfo.allocationSize = memReqs.size;
                memAllocInfo.memoryTypeIndex = vulkanDevice->getMemoryType(memReqs.memoryTypeBits, VK_MEMORY_PROPERTY_Device_LOCAL_BIT);

                Util.CheckResult(vkAllocateMemory(Device, &memAllocInfo, null, &texture.DeviceMemory));
                Util.CheckResult(vkBindImageMemory(Device, texture.image, texture.DeviceMemory, 0));

                VkCommandBuffer copyCmd = VulkanExampleBase::createCommandBuffer(VK_COMMAND_BUFFER_LEVEL_PRIMARY, true);

                // Image barrier for optimal image

                // The sub resource range describes the regions of the image we will be transition
                VkImageSubresourceRange subresourceRange = { };
                // Image only contains color data
                subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
                // Start at first mip level
                subresourceRange.baseMipLevel = 0;
                // We will transition on all mip levels
                subresourceRange.levelCount = texture.mipLevels;
                // The 2D texture only has one layer
                subresourceRange.layerCount = 1;

                // Optimal image will be used as destination for the copy, so we must transfer from our
                // initial undefined image layout to the transfer destination layout
                setImageLayout(
                    copyCmd,
                    texture.image,
                    VK_IMAGE_ASPECT_COLOR_BIT,
                    VK_IMAGE_LAYOUT_UNDEFINED,
                    VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                    subresourceRange);

                // Copy mip levels from staging buffer
                vkCmdCopyBufferToImage(
                    copyCmd,
                    stagingBuffer,
                    texture.image,
                    VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                    static_cast<uint>(bufferCopyRegions.size()),
                    bufferCopyRegions.data());

                // Change texture image layout to shader read after all mip levels have been copied
                texture.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
                setImageLayout(
                    copyCmd,
                    texture.image,
                    VK_IMAGE_ASPECT_COLOR_BIT,
                    VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                    texture.imageLayout,
                    subresourceRange);

                VulkanExampleBase::flushCommandBuffer(copyCmd, queue, true);

                // Clean up staging resources
                vkFreeMemory(Device, stagingMemory, null);
                vkDestroyBuffer(Device, stagingBuffer, null);
            }
            else
            {
                // Prefer using optimal tiling, as linear tiling 
                // may support only a small set of features 
                // depending on implementation (e.g. no mip maps, only one layer, etc.)

                VkImage mappableImage;
                VkDeviceMemory mappableMemory;

                // Load mip map level 0 to linear tiling image
                VkImageCreateInfo imageCreateInfo = vks::initializers::imageCreateInfo();
                imageCreateInfo.imageType = VK_IMAGE_TYPE_2D;
                imageCreateInfo.format = format;
                imageCreateInfo.mipLevels = 1;
                imageCreateInfo.arrayLayers = 1;
                imageCreateInfo.samples = VK_SAMPLE_COUNT_1_BIT;
                imageCreateInfo.tiling = VK_IMAGE_TILING_LINEAR;
                imageCreateInfo.usage = VK_IMAGE_USAGE_SAMPLED_BIT;
                imageCreateInfo.sharingMode = VK_SHARING_MODE_EXCLUSIVE;
                imageCreateInfo.initialLayout = VK_IMAGE_LAYOUT_PREINITIALIZED;
                imageCreateInfo.extent = { texture.width, texture.height, 1 };
                Util.CheckResult(vkCreateImage(Device, &imageCreateInfo, null, &mappableImage));

                // Get memory requirements for this image 
                // like size and alignment
                vkGetImageMemoryRequirements(Device, mappableImage, &memReqs);
                // Set memory allocation size to required memory size
                memAllocInfo.allocationSize = memReqs.size;

                // Get memory type that can be mapped to host memory
                memAllocInfo.memoryTypeIndex = vulkanDevice->getMemoryType(memReqs.memoryTypeBits, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);

                // Allocate host memory
                Util.CheckResult(vkAllocateMemory(Device, &memAllocInfo, null, &mappableMemory));

                // Bind allocated image for use
                Util.CheckResult(vkBindImageMemory(Device, mappableImage, mappableMemory, 0));

                // Get sub resource layout
                // Mip map count, array layer, etc.
                VkImageSubresource subRes = { };
                subRes.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;

                VkSubresourceLayout subResLayout;
                void* data;

                // Get sub resources layout 
                // Includes row pitch, size offsets, etc.
                vkGetImageSubresourceLayout(Device, mappableImage, &subRes, &subResLayout);

                // Map image memory
                Util.CheckResult(vkMapMemory(Device, mappableMemory, 0, memReqs.size, 0, &data));

                // Copy image data into memory
                memcpy(data, tex2D[subRes.mipLevel].data(), tex2D[subRes.mipLevel].size());

                vkUnmapMemory(Device, mappableMemory);

                // Linear tiled images don't need to be staged
                // and can be directly used as textures
                texture.image = mappableImage;
                texture.DeviceMemory = mappableMemory;
                texture.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;

                VkCommandBuffer copyCmd = VulkanExampleBase::createCommandBuffer(VK_COMMAND_BUFFER_LEVEL_PRIMARY, true);

                // Setup image memory barrier transfer image to shader read layout

                // The sub resource range describes the regions of the image we will be transition
                VkImageSubresourceRange subresourceRange = { };
                // Image only contains color data
                subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
                // Start at first mip level
                subresourceRange.baseMipLevel = 0;
                // Only one mip level, most implementations won't support more for linear tiled images
                subresourceRange.levelCount = 1;
                // The 2D texture only has one layer
                subresourceRange.layerCount = 1;

                setImageLayout(
                    copyCmd,
                    texture.image,
                    VK_IMAGE_ASPECT_COLOR_BIT,
                    VK_IMAGE_LAYOUT_PREINITIALIZED,
                    texture.imageLayout,
                    subresourceRange);

                VulkanExampleBase::flushCommandBuffer(copyCmd, queue, true);
            }

            // Create sampler
            // In Vulkan textures are accessed by samplers
            // This separates all the sampling information from the 
            // texture data
            // This means you could have multiple sampler objects
            // for the same texture with different settings
            // Similar to the samplers available with OpenGL 3.3
            VkSamplerCreateInfo sampler = vks::initializers::samplerCreateInfo();
            sampler.magFilter = VK_FILTER_LINEAR;
            sampler.minFilter = VK_FILTER_LINEAR;
            sampler.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
            sampler.addressModeU = VK_SAMPLER_ADDRESS_MODE_REPEAT;
            sampler.addressModeV = VK_SAMPLER_ADDRESS_MODE_REPEAT;
            sampler.addressModeW = VK_SAMPLER_ADDRESS_MODE_REPEAT;
            sampler.mipLodBias = 0.0f;
            sampler.compareOp = VK_COMPARE_OP_NEVER;
            sampler.minLod = 0.0f;
            // Set max level-of-detail to mip level count of the texture
            sampler.maxLod = (useStaging) ? (float)texture.mipLevels : 0.0f;
            // Enable anisotropic filtering
            // This feature is optional, so we must check if it's supported on the Device
            if (vulkanDevice->features.samplerAnisotropy)
            {
                // Use max. level of anisotropy for this example
                sampler.maxAnisotropy = vulkanDevice->properties.limits.maxSamplerAnisotropy;
                sampler.anisotropyEnable = VK_TRUE;
            }
            else
            {
                // The Device does not support anisotropic filtering
                sampler.maxAnisotropy = 1.0;
                sampler.anisotropyEnable = VK_FALSE;
            }
            sampler.borderColor = VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE;
            Util.CheckResult(vkCreateSampler(Device, &sampler, null, &texture.sampler));

            // Create image view
            // Textures are not directly accessed by the shaders and
            // are abstracted by image views containing additional
            // information and sub resource ranges
            VkImageViewCreateInfo view = vks::initializers::imageViewCreateInfo();
            view.viewType = VK_IMAGE_VIEW_TYPE_2D;
            view.format = format;
            view.components = { VK_COMPONENT_SWIZZLE_R, VK_COMPONENT_SWIZZLE_G, VK_COMPONENT_SWIZZLE_B, VK_COMPONENT_SWIZZLE_A };
            // The subresource range describes the set of mip levels (and array layers) that can be accessed through this image view
            // It's possible to create multiple image views for a single image referring to different (and/or overlapping) ranges of the image
            view.subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
            view.subresourceRange.baseMipLevel = 0;
            view.subresourceRange.baseArrayLayer = 0;
            view.subresourceRange.layerCount = 1;
            // Linear tiling usually won't support mip maps
            // Only set mip map count if optimal tiling is used
            view.subresourceRange.levelCount = (useStaging) ? texture.mipLevels : 1;
            // The view will be based on the texture's image
            view.image = texture.image;
            Util.CheckResult(vkCreateImageView(Device, &view, null, &texture.view));
        }

        // Free all Vulkan resources used a texture object
        void destroyTextureImage(Texture texture)
        {
            vkDestroyImageView(Device, texture.view, null);
            vkDestroyImage(Device, texture.image, null);
            vkDestroySampler(Device, texture.sampler, null);
            vkFreeMemory(Device, texture.DeviceMemory, null);
        }

        void loadTextures()
        {
            // Vulkan core supports three different compressed texture formats
            // As the support differs between implemementations we need to check Device features and select a proper format and file
            std::string filename;
            VkFormat format;
            if (DeviceFeatures.textureCompressionBC)
            {
                filename = "metalplate01_bc2_unorm.ktx";
                format = VK_FORMAT_BC2_UNORM_BLOCK;
            }
            else if (DeviceFeatures.textureCompressionASTC_LDR)
            {
                filename = "metalplate01_astc_8x8_unorm.ktx";
                format = VK_FORMAT_ASTC_8x8_UNORM_BLOCK;
            }
            else if (DeviceFeatures.textureCompressionETC2)
            {
                filename = "metalplate01_etc2_unorm.ktx";
                format = VK_FORMAT_ETC2_R8G8B8A8_UNORM_BLOCK;
            }
            else
            {
                vks::tools::exitFatal("Device does not support any compressed texture format!", "Error");
            }

            loadTexture(getAssetPath() + "textures/" + filename, format, false);
        }

        void buildCommandBuffers()
        {
            VkCommandBufferBeginInfo cmdBufInfo = vks::initializers::commandBufferBeginInfo();

            VkClearValue clearValues[2];
            clearValues[0].color = defaultClearColor;
            clearValues[1].depthStencil = { 1.0f, 0 };

            VkRenderPassBeginInfo renderPassBeginInfo = vks::initializers::renderPassBeginInfo();
            renderPassBeginInfo.renderPass = renderPass;
            renderPassBeginInfo.renderArea.offset.x = 0;
            renderPassBeginInfo.renderArea.offset.y = 0;
            renderPassBeginInfo.renderArea.extent.width = width;
            renderPassBeginInfo.renderArea.extent.height = height;
            renderPassBeginInfo.clearValueCount = 2;
            renderPassBeginInfo.pClearValues = clearValues;

            for (int32_t i = 0; i < drawCmdBuffers.size(); ++i)
            {
                // Set target frame buffer
                renderPassBeginInfo.framebuffer = frameBuffers[i];

                Util.CheckResult(vkBeginCommandBuffer(drawCmdBuffers[i], &cmdBufInfo));

                vkCmdBeginRenderPass(drawCmdBuffers[i], &renderPassBeginInfo, VK_SUBPASS_CONTENTS_INLINE);

                VkViewport viewport = vks::initializers::viewport((float)width, (float)height, 0.0f, 1.0f);
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);

                VkRect2D scissor = vks::initializers::rect2D(width, height, 0, 0);
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, &scissor);

                vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayout, 0, 1, &descriptorSet, 0, NULL);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_solid);

                VkDeviceSize offsets[1] = { 0 };
                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, &vertexBuffer.buffer, offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], indexBuffer.buffer, 0, VK_INDEX_TYPE_UINT32);

                vkCmdDrawIndexed(drawCmdBuffers[i], indexCount, 1, 0, 0, 0);

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void draw()
        {
            VulkanExampleBase::prepareFrame();

            // Command buffer to be sumitted to the queue
            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = &drawCmdBuffers[currentBuffer];

            // Submit to queue
            Util.CheckResult(vkQueueSubmit(queue, 1, &submitInfo, VK_NULL_HANDLE));

            VulkanExampleBase::submitFrame();
        }

        void generateQuad()
        {
            // Setup vertices for a single uv-mapped quad made from two triangles
            std::vector<Vertex> vertices =
            {
                    { {  1.0f,  1.0f, 0.0f }, { 1.0f, 1.0f },{ 0.0f, 0.0f, 1.0f } },
                    { { -1.0f,  1.0f, 0.0f }, { 0.0f, 1.0f },{ 0.0f, 0.0f, 1.0f } },
                    { { -1.0f, -1.0f, 0.0f }, { 0.0f, 0.0f },{ 0.0f, 0.0f, 1.0f } },
                    { {  1.0f, -1.0f, 0.0f }, { 1.0f, 0.0f },{ 0.0f, 0.0f, 1.0f } }
                };

            // Setup indices
            std::vector<uint> indices = { 0, 1, 2, 2, 3, 0 };
            indexCount = static_cast<uint>(indices.size());

            // Create buffers
            // For the sake of simplicity we won't stage the vertex data to the gpu memory
            // Vertex buffer
            Util.CheckResult(vulkanDevice->createBuffer(
                VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                &vertexBuffer,
                vertices.size() * sizeof(Vertex),
                vertices.data()));
            // Index buffer
            Util.CheckResult(vulkanDevice->createBuffer(
                VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                &indexBuffer,
                indices.size() * sizeof(uint),
                indices.data()));
        }

        void setupVertexDescriptions()
        {
            // Binding description
            vertices.bindingDescriptions.resize(1);
            vertices.bindingDescriptions[0] =
                vks::initializers::vertexInputBindingDescription(
                    VERTEX_BUFFER_BIND_ID,
                    sizeof(Vertex),
                    VK_VERTEX_INPUT_RATE_VERTEX);

            // Attribute descriptions
            // Describes memory layout and shader positions
            vertices.attributeDescriptions.resize(3);
            // Location 0 : Position
            vertices.attributeDescriptions[0] =
                vks::initializers::vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    0,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    offsetof(Vertex, pos));
            // Location 1 : Texture coordinates
            vertices.attributeDescriptions[1] =
                vks::initializers::vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    1,
                    VK_FORMAT_R32G32_SFLOAT,
                    offsetof(Vertex, uv));
            // Location 1 : Vertex normal
            vertices.attributeDescriptions[2] =
                vks::initializers::vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    2,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    offsetof(Vertex, normal));

            vertices.inputState = vks::initializers::pipelineVertexInputStateCreateInfo();
            vertices.inputState.vertexBindingDescriptionCount = static_cast<uint>(vertices.bindingDescriptions.size());
            vertices.inputState.pVertexBindingDescriptions = vertices.bindingDescriptions.data();
            vertices.inputState.vertexAttributeDescriptionCount = static_cast<uint>(vertices.attributeDescriptions.size());
            vertices.inputState.pVertexAttributeDescriptions = vertices.attributeDescriptions.data();
        }

        void setupDescriptorPool()
        {
            // Example uses one ubo and one image sampler
            std::vector<VkDescriptorPoolSize> poolSizes =
            {
                    vks::initializers::descriptorPoolSize(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 1),
                    vks::initializers::descriptorPoolSize(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, 1)
                };

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                vks::initializers::descriptorPoolCreateInfo(
                    static_cast<uint>(poolSizes.size()),
                    poolSizes.data(),
                    2);

            Util.CheckResult(vkCreateDescriptorPool(Device, &descriptorPoolInfo, null, &descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            std::vector<VkDescriptorSetLayoutBinding> setLayoutBindings =
            {
        			// Binding 0 : Vertex shader uniform buffer
        			vks::initializers::descriptorSetLayoutBinding(
                        VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                        VK_SHADER_STAGE_VERTEX_BIT,
                        0),
        			// Binding 1 : Fragment shader image sampler
        			vks::initializers::descriptorSetLayoutBinding(
                        VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                        VK_SHADER_STAGE_FRAGMENT_BIT,
                        1)
                };

            VkDescriptorSetLayoutCreateInfo descriptorLayout =
                vks::initializers::descriptorSetLayoutCreateInfo(
                    setLayoutBindings.data(),
                    static_cast<uint>(setLayoutBindings.size()));

            Util.CheckResult(vkCreateDescriptorSetLayout(Device, &descriptorLayout, null, &descriptorSetLayout));

            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo =
                vks::initializers::pipelineLayoutCreateInfo(
                    &descriptorSetLayout,
                    1);

            Util.CheckResult(vkCreatePipelineLayout(Device, &pPipelineLayoutCreateInfo, null, &pipelineLayout));
        }

        void setupDescriptorSet()
        {
            VkDescriptorSetAllocateInfo allocInfo =
                vks::initializers::descriptorSetAllocateInfo(
                    descriptorPool,
                    &descriptorSetLayout,
                    1);

            Util.CheckResult(vkAllocateDescriptorSets(Device, &allocInfo, &descriptorSet));

            // Setup a descriptor image info for the current texture to be used as a combined image sampler
            VkDescriptorImageInfo textureDescriptor;
            textureDescriptor.imageView = texture.view;             // The image's view (images are never directly accessed by the shader, but rather through views defining subresources)
            textureDescriptor.sampler = texture.sampler;            //	The sampler (Telling the pipeline how to sample the texture, including repeat, border, etc.)
            textureDescriptor.imageLayout = texture.imageLayout;    //	The current layout of the image (Note: Should always fit the actual use, e.g. shader read)

            std::vector<VkWriteDescriptorSet> writeDescriptorSets =
            {
        			// Binding 0 : Vertex shader uniform buffer
        			vks::initializers::writeDescriptorSet(
                        descriptorSet,
                        VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                        0,
                        &uniformBufferVS.descriptor),
        			// Binding 1 : Fragment shader texture sampler
        			//	Fragment shader: layout (binding = 1) uniform sampler2D samplerColor;
        			vks::initializers::writeDescriptorSet(
                        descriptorSet,
                        VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,			// The descriptor set will use a combined image sampler (sampler and image could be split)
        				1,													// Shader binding point 1
        				&textureDescriptor)								// Pointer to the descriptor image for our texture
        		};

            vkUpdateDescriptorSets(Device, static_cast<uint>(writeDescriptorSets.size()), writeDescriptorSets.data(), 0, NULL);
        }

        void preparePipelines()
        {
            VkPipelineInputAssemblyStateCreateInfo inputAssemblyState =
                vks::initializers::pipelineInputAssemblyStateCreateInfo(
                    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                    0,
                    VK_FALSE);

            VkPipelineRasterizationStateCreateInfo rasterizationState =
                vks::initializers::pipelineRasterizationStateCreateInfo(
                    VK_POLYGON_MODE_FILL,
                    VK_CULL_MODE_NONE,
                    VK_FRONT_FACE_COUNTER_CLOCKWISE,
                    0);

            VkPipelineColorBlendAttachmentState blendAttachmentState =
                vks::initializers::pipelineColorBlendAttachmentState(
                    0xf,
                    VK_FALSE);

            VkPipelineColorBlendStateCreateInfo colorBlendState =
                vks::initializers::pipelineColorBlendStateCreateInfo(
                    1,
                    &blendAttachmentState);

            VkPipelineDepthStencilStateCreateInfo depthStencilState =
                vks::initializers::pipelineDepthStencilStateCreateInfo(
                    VK_TRUE,
                    VK_TRUE,
                    VK_COMPARE_OP_LESS_OR_EQUAL);

            VkPipelineViewportStateCreateInfo viewportState =
                vks::initializers::pipelineViewportStateCreateInfo(1, 1, 0);

            VkPipelineMultisampleStateCreateInfo multisampleState =
                vks::initializers::pipelineMultisampleStateCreateInfo(
                    VK_SAMPLE_COUNT_1_BIT,
                    0);

            std::vector<VkDynamicState> dynamicStateEnables = {
                    VK_DYNAMIC_STATE_VIEWPORT,
                    VK_DYNAMIC_STATE_SCISSOR
                };
            VkPipelineDynamicStateCreateInfo dynamicState =
                vks::initializers::pipelineDynamicStateCreateInfo(
                    dynamicStateEnables.data(),
                    static_cast<uint>(dynamicStateEnables.size()),
                    0);

            // Load shaders
            std::array < VkPipelineShaderStageCreateInfo,2 > shaderStages;

            shaderStages[0] = loadShader(getAssetPath() + "shaders/texture/texture.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages[1] = loadShader(getAssetPath() + "shaders/texture/texture.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);

            VkGraphicsPipelineCreateInfo pipelineCreateInfo =
                vks::initializers::pipelineCreateInfo(
                    pipelineLayout,
                    renderPass,
                    0);

            pipelineCreateInfo.pVertexInputState = &vertices.inputState;
            pipelineCreateInfo.pInputAssemblyState = &inputAssemblyState;
            pipelineCreateInfo.pRasterizationState = &rasterizationState;
            pipelineCreateInfo.pColorBlendState = &colorBlendState;
            pipelineCreateInfo.pMultisampleState = &multisampleState;
            pipelineCreateInfo.pViewportState = &viewportState;
            pipelineCreateInfo.pDepthStencilState = &depthStencilState;
            pipelineCreateInfo.pDynamicState = &dynamicState;
            pipelineCreateInfo.stageCount = static_cast<uint>(shaderStages.size());
            pipelineCreateInfo.pStages = shaderStages.data();

            Util.CheckResult(vkCreateGraphicsPipelines(Device, pipelineCache, 1, &pipelineCreateInfo, null, &pipelines_solid));
        }

        // Prepare and initialize uniform buffer containing shader uniforms
        void prepareUniformBuffers()
        {
            // Vertex shader uniform buffer block
            Util.CheckResult(vulkanDevice->createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                &uniformBufferVS,
                sizeof(uboVS),
                &uboVS));

            updateUniformBuffers();
        }

        void updateUniformBuffers()
        {
            // Vertex shader
            uboVS.projection = glm::perspective(glm::radians(60.0f), (float)width / (float)height, 0.001f, 256.0f);
            Matrix4x4 viewMatrix = glm::translate(Matrix4x4(), glm::vec3(0.0f, 0.0f, zoom));

            uboVS.model = viewMatrix * glm::translate(Matrix4x4(), cameraPos);
            uboVS.model = glm::rotate(uboVS.model, glm::radians(rotation.x), glm::vec3(1.0f, 0.0f, 0.0f));
            uboVS.model = glm::rotate(uboVS.model, glm::radians(rotation.y), glm::vec3(0.0f, 1.0f, 0.0f));
            uboVS.model = glm::rotate(uboVS.model, glm::radians(rotation.z), glm::vec3(0.0f, 0.0f, 1.0f));

            uboVS.viewPos = vector4(0.0f, 0.0f, -zoom, 0.0f);

            Util.CheckResult(uniformBufferVS.map());
            memcpy(uniformBufferVS.mapped, &uboVS, sizeof(uboVS));
            uniformBufferVS.unmap();
        }

        void prepare()
        {
            VulkanExampleBase::prepare();
            loadTextures();
            generateQuad();
            setupVertexDescriptions();
            prepareUniformBuffers();
            setupDescriptorSetLayout();
            preparePipelines();
            setupDescriptorPool();
            setupDescriptorSet();
            buildCommandBuffers();
            prepared = true;
        }

        virtual void render()
        {
            if (!prepared)
                return;
            draw();
        }

        virtual void viewChanged()
        {
            updateUniformBuffers();
        }

        void changeLodBias(float delta)
        {
            uboVS.lodBias += delta;
            if (uboVS.lodBias < 0.0f)
            {
                uboVS.lodBias = 0.0f;
            }
            if (uboVS.lodBias > texture.mipLevels)
            {
                uboVS.lodBias = (float)texture.mipLevels;
            }
            updateUniformBuffers();
            updateTextOverlay();
        }

        virtual void keyPressed(uint keyCode)
        {
            switch (keyCode)
            {
                case KEY_KPADD:
                case GAMEPAD_BUTTON_R1:
                    changeLodBias(0.1f);
                    break;
                case KEY_KPSUB:
                case GAMEPAD_BUTTON_L1:
                    changeLodBias(-0.1f);
                    break;
            }
        }

        virtual void getOverlayText(VulkanTextOverlay* textOverlay)
        {
            std::stringstream ss;
            ss << std::setprecision(2) << std::fixed << uboVS.lodBias;
            textOverlay->addText("LOD bias: " + ss.str() + " (numpad +/- to change)", 5.0f, 85.0f, VulkanTextOverlay::alignLeft);
        }

        public static void Main() => new TextureMappingExample().ExampleMain();
    }
}