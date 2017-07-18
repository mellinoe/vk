// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: specializationconstants/specializationconstants.cpp

/*
* Vulkan Example - Texture arrays and instanced rendering
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid.Collections;
using Vulkan;
using static Vulkan.VulkanNative;
using static Vulkan.RawConstants;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace Vk.Samples
{
    public unsafe class TextureArrayExample : VulkanExampleBase
    {
        private const uint VERTEX_BUFFER_BIND_ID = 0;

        // Vertex layout for this example
        struct Vertex
        {
            public Vector3 pos;
            public Vector2 uv;
        };

        // Number of array layers in texture array
        // Also used as instance count
        uint layerCount;
        vksTexture textureArray = new vksTexture();

        VkPipelineVertexInputStateCreateInfo vertices_inputState;
        NativeList<VkVertexInputBindingDescription> vertices_bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
        NativeList<VkVertexInputAttributeDescription> vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();

        vksBuffer vertexBuffer = new vksBuffer();
        vksBuffer indexBuffer = new vksBuffer();
        uint indexCount;

        vksBuffer uniformBufferVS = new vksBuffer();

        struct UboInstanceData
        {
            // Model matrix
            public Matrix4x4 model;
            // Texture array index
            // Vec4 due to padding
            public Vector4 arrayIndex;
        }

        struct UboVS
        {
            public struct UboVS_Matrices
            {
                public Matrix4x4 projection;
                public Matrix4x4 view;
            }

            // Global matrices
            public UboVS_Matrices matrices;
            // Seperate data for each instance
            public UboInstanceData* instance;
        }

        UboVS uboVS;

        VkPipeline pipeline;
        VkPipelineLayout pipelineLayout;
        VkDescriptorSet descriptorSet;
        VkDescriptorSetLayout descriptorSetLayout;

        public TextureArrayExample()
        {
            zoom = -15.0f;
            rotationSpeed = 0.25f;
            rotation = new Vector3(-15.0f, 35.0f, 0.0f);
            enableTextOverlay = true;
            title = "Vulkan Example - Texture arrays";
        }

        public void Dispose()
        {
            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class

            // Clean up texture resources
            vkDestroyImageView(device, textureArray.view, null);
            vkDestroyImage(device, textureArray.image, null);
            vkDestroySampler(device, textureArray.sampler, null);
            vkFreeMemory(device, textureArray.deviceMemory, null);

            vkDestroyPipeline(device, pipeline, null);

            vkDestroyPipelineLayout(device, pipelineLayout, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayout, null);

            vertexBuffer.destroy();
            indexBuffer.destroy();

            uniformBufferVS.destroy();

            // delete[] uboVS.instance;
        }

        void loadTextureArray(string filename, VkFormat format)
        {
            KtxFile tex2DArray;
            using (var fs = File.OpenRead(filename))
            {
                tex2DArray = KtxFile.Load(fs, false);
            }

            textureArray.width = tex2DArray.Header.PixelWidth;
            textureArray.height = tex2DArray.Header.PixelHeight;
            layerCount = tex2DArray.Header.NumberOfArrayElements;

            VkMemoryAllocateInfo memAllocInfo = Initializers.memoryAllocateInfo();
            VkMemoryRequirements memReqs;

            // Create a host-visible staging buffer that contains the raw image data
            VkBuffer stagingBuffer;
            VkDeviceMemory stagingMemory;

            VkBufferCreateInfo bufferCreateInfo = Initializers.bufferCreateInfo();
            bufferCreateInfo.size = tex2DArray.GetTotalSize();
            // This buffer is used as a transfer source for the buffer copy
            bufferCreateInfo.usage = VK_BUFFER_USAGE_TRANSFER_SRC_BIT;
            bufferCreateInfo.sharingMode = VK_SHARING_MODE_EXCLUSIVE;

            Util.CheckResult(vkCreateBuffer(device, &bufferCreateInfo, null, &stagingBuffer));

            // Get memory requirements for the staging buffer (alignment, memory type bits)
            vkGetBufferMemoryRequirements(device, stagingBuffer, &memReqs);

            memAllocInfo.allocationSize = memReqs.size;
            // Get memory type index for a host visible buffer
            memAllocInfo.memoryTypeIndex = vulkanDevice.getMemoryType(memReqs.memoryTypeBits, VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT);

            Util.CheckResult(vkAllocateMemory(device, &memAllocInfo, null, &stagingMemory));
            Util.CheckResult(vkBindBufferMemory(device, stagingBuffer, stagingMemory, 0));

            // Copy texture data into staging buffer
            byte* data;
            Util.CheckResult(vkMapMemory(device, stagingMemory, 0, memReqs.size, 0, (void**)&data));
            byte[] allTextureData = tex2DArray.GetAllTextureData();
            fixed (byte* texPtr = allTextureData)
            {
                Unsafe.CopyBlock(data, texPtr, (uint)allTextureData.Length);
            }
            vkUnmapMemory(device, stagingMemory);

            // Setup buffer copy regions for array layers
            NativeList<VkBufferImageCopy> bufferCopyRegions;
            IntPtr offset = IntPtr.Zero;

            for (uint layer = 0; layer < layerCount; layer++)
            {
                VkBufferImageCopy bufferCopyRegion = new VkBufferImageCopy();
                bufferCopyRegion.imageSubresource.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
                bufferCopyRegion.imageSubresource.mipLevel = 0;
                bufferCopyRegion.imageSubresource.baseArrayLayer = layer;
                bufferCopyRegion.imageSubresource.layerCount = 1;
                bufferCopyRegion.imageExtent.width = (uint)(tex2DArray[layer][0].extent().x);
                bufferCopyRegion.imageExtent.height = (uint)(tex2DArray[layer][0].extent().y);
                bufferCopyRegion.imageExtent.depth = 1;
                bufferCopyRegion.bufferOffset = offset;

                bufferCopyRegions.push_back(bufferCopyRegion);

                // Increase offset into staging buffer for next level / face
                offset += tex2DArray[layer][0].Count;
            }

            // Create optimal tiled target image
            VkImageCreateInfo imageCreateInfo = Initializers.imageCreateInfo();
            imageCreateInfo.imageType = VK_IMAGE_TYPE_2D;
            imageCreateInfo.format = format;
            imageCreateInfo.mipLevels = 1;
            imageCreateInfo.samples = VK_SAMPLE_COUNT_1_BIT;
            imageCreateInfo.tiling = VK_IMAGE_TILING_OPTIMAL;
            imageCreateInfo.usage = VK_IMAGE_USAGE_SAMPLED_BIT;
            imageCreateInfo.sharingMode = VK_SHARING_MODE_EXCLUSIVE;
            imageCreateInfo.initialLayout = VK_IMAGE_LAYOUT_UNDEFINED;
            imageCreateInfo.extent = new  { textureArray.width, textureArray.height, 1 };
            imageCreateInfo.usage = VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT;
            imageCreateInfo.arrayLayers = layerCount;

            Util.CheckResult(vkCreateImage(device, &imageCreateInfo, null, &textureArray.image));

            vkGetImageMemoryRequirements(device, textureArray.image, &memReqs);

            memAllocInfo.allocationSize = memReqs.size;
            memAllocInfo.memoryTypeIndex = vulkanDevice->getMemoryType(memReqs.memoryTypeBits, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);

            Util.CheckResult(vkAllocateMemory(device, &memAllocInfo, null, &textureArray.deviceMemory));
            Util.CheckResult(vkBindImageMemory(device, textureArray.image, textureArray.deviceMemory, 0));

            VkCommandBuffer copyCmd = VulkanExampleBase::createCommandBuffer(VK_COMMAND_BUFFER_LEVEL_PRIMARY, true);

            // Image barrier for optimal image (target)
            // Set initial layout for all array layers (faces) of the optimal (target) tiled texture
            VkImageSubresourceRange subresourceRange = { };
            subresourceRange.aspectMask = VK_IMAGE_ASPECT_COLOR_BIT;
            subresourceRange.baseMipLevel = 0;
            subresourceRange.levelCount = 1;
            subresourceRange.layerCount = layerCount;

            vkstools::setImageLayout(
                copyCmd,
                textureArray.image,
                VK_IMAGE_ASPECT_COLOR_BIT,
                VK_IMAGE_LAYOUT_UNDEFINED,
                VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                subresourceRange);

            // Copy the cube map faces from the staging buffer to the optimal tiled image
            vkCmdCopyBufferToImage(
                copyCmd,
                stagingBuffer,
                textureArray.image,
                VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                bufferCopyRegions.Count,
                bufferCopyRegions.Data
                );

            // Change texture image layout to shader read after all faces have been copied
            textureArray.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
            vkstools::setImageLayout(
                copyCmd,
                textureArray.image,
                VK_IMAGE_ASPECT_COLOR_BIT,
                VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                textureArray.imageLayout,
                subresourceRange);

            VulkanExampleBase::flushCommandBuffer(copyCmd, queue, true);

            // Create sampler
            VkSamplerCreateInfo sampler = Initializers.samplerCreateInfo();
            sampler.magFilter = VK_FILTER_LINEAR;
            sampler.minFilter = VK_FILTER_LINEAR;
            sampler.mipmapMode = VK_SAMPLER_MIPMAP_MODE_LINEAR;
            sampler.addressModeU = VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE;
            sampler.addressModeV = sampler.addressModeU;
            sampler.addressModeW = sampler.addressModeU;
            sampler.mipLodBias = 0.0f;
            sampler.maxAnisotropy = 8;
            sampler.compareOp = VK_COMPARE_OP_NEVER;
            sampler.minLod = 0.0f;
            sampler.maxLod = 0.0f;
            sampler.borderColor = VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE;
            Util.CheckResult(vkCreateSampler(device, &sampler, null, &textureArray.sampler));

            // Create image view
            VkImageViewCreateInfo view = Initializers.imageViewCreateInfo();
            view.viewType = VK_IMAGE_VIEW_TYPE_2D_ARRAY;
            view.format = format;
            view.components = { VK_COMPONENT_SWIZZLE_R, VK_COMPONENT_SWIZZLE_G, VK_COMPONENT_SWIZZLE_B, VK_COMPONENT_SWIZZLE_A };
            view.subresourceRange = { VK_IMAGE_ASPECT_COLOR_BIT, 0, 1, 0, 1 };
            view.subresourceRange.layerCount = layerCount;
            view.subresourceRange.levelCount = 1;
            view.image = textureArray.image;
            Util.CheckResult(vkCreateImageView(device, &view, null, &textureArray.view));

            // Clean up staging resources
            vkFreeMemory(device, stagingMemory, null);
            vkDestroyBuffer(device, stagingBuffer, null);
        }

        void loadTextures()
        {
            // Vulkan core supports three different compressed texture formats
            // As the support differs between implemementations we need to check device features and select a proper format and file
            std::string filename;
            VkFormat format;
            if (deviceFeatures.textureCompressionBC)
            {
                filename = "texturearray_bc3_unorm.ktx";
                format = VK_FORMAT_BC3_UNORM_BLOCK;
            }
            else if (deviceFeatures.textureCompressionASTC_LDR)
            {
                filename = "texturearray_astc_8x8_unorm.ktx";
                format = VK_FORMAT_ASTC_8x8_UNORM_BLOCK;
            }
            else if (deviceFeatures.textureCompressionETC2)
            {
                filename = "texturearray_etc2_unorm.ktx";
                format = VK_FORMAT_ETC2_R8G8B8_UNORM_BLOCK;
            }
            else
            {
                vkstools::exitFatal("Device does not support any compressed texture format!", "Error");
            }
            loadTextureArray(getAssetPath() + "textures/" + filename, format);
        }

        void buildCommandBuffers()
        {
            VkCommandBufferBeginInfo cmdBufInfo = Initializers.commandBufferBeginInfo();

            VkClearValue clearValues[2];
            clearValues[0].color = defaultClearColor;
            clearValues[1].depthStencil = { 1.0f, 0 };

            VkRenderPassBeginInfo renderPassBeginInfo = Initializers.renderPassBeginInfo();
            renderPassBeginInfo.renderPass = renderPass;
            renderPassBeginInfo.renderArea.offset.x = 0;
            renderPassBeginInfo.renderArea.offset.y = 0;
            renderPassBeginInfo.renderArea.extent.width = width;
            renderPassBeginInfo.renderArea.extent.height = height;
            renderPassBeginInfo.clearValueCount = 2;
            renderPassBeginInfo.pClearValues = clearValues;

            for (int32_t i = 0; i < drawCmdBuffers.Count; ++i)
            {
                // Set target frame buffer
                renderPassBeginInfo.framebuffer = frameBuffers[i];

                Util.CheckResult(vkBeginCommandBuffer(drawCmdBuffers[i], &cmdBufInfo));

                vkCmdBeginRenderPass(drawCmdBuffers[i], &renderPassBeginInfo, VK_SUBPASS_CONTENTS_INLINE);

                VkViewport viewport = Initializers.viewport((float)width, (float)height, 0.0f, 1.0f);
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);

                VkRect2D scissor = Initializers.rect2D(width, height, 0, 0);
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, &scissor);

                vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayout, 0, 1, &descriptorSet, 0, NULL);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipeline);

                VkDeviceSize offsets[1] = { 0 };
                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, &vertexBuffer.buffer, offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], indexBuffer.buffer, 0, VK_INDEX_TYPE_UINT32);

                vkCmdDrawIndexed(drawCmdBuffers[i], indexCount, layerCount, 0, 0, 0);

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void generateQuad()
        {
            // Setup vertices for a single uv-mapped quad made from two triangles
            NativeList<Vertex> vertices =
            {
            { {  2.5f,  2.5f, 0.0f }, { 1.0f, 1.0f } },
            { { -2.5f,  2.5f, 0.0f }, { 0.0f, 1.0f } },
            { { -2.5f, -2.5f, 0.0f }, { 0.0f, 0.0f } },
            { {  2.5f, -2.5f, 0.0f }, { 1.0f, 0.0f } }
        };

            // Setup indices
            NativeList<uint> indices = { 0, 1, 2, 2, 3, 0 };
            indexCount = (uint)(indices.Count);

            // Create buffers
            // For the sake of simplicity we won't stage the vertex data to the gpu memory
            // Vertex buffer
            Util.CheckResult(vulkanDevice->createBuffer(
                VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                &vertexBuffer,
                vertices.Count * sizeof(Vertex),
                vertices.Data));
            // Index buffer
            Util.CheckResult(vulkanDevice->createBuffer(
                VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                &indexBuffer,
                indices.Count * sizeof(uint),
                indices.Data));
        }

        void setupVertexDescriptions()
        {
            // Binding description
            vertices.bindingDescriptions.resize(1);
            vertices.bindingDescriptions[0] =
                Initializers.vertexInputBindingDescription(
                    VERTEX_BUFFER_BIND_ID,
                    sizeof(Vertex),
                    VK_VERTEX_INPUT_RATE_VERTEX);

            // Attribute descriptions
            // Describes memory layout and shader positions
            vertices.attributeDescriptions.resize(2);
            // Location 0 : Position
            vertices.attributeDescriptions[0] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    0,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    0);
            // Location 1 : Texture coordinates
            vertices.attributeDescriptions[1] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    1,
                    VK_FORMAT_R32G32_SFLOAT,
                    sizeof(float) * 3);

            vertices.inputState = Initializers.pipelineVertexInputStateCreateInfo();
            vertices.inputState.vertexBindingDescriptionCount = vertices.bindingDescriptions.Count;
            vertices.inputState.pVertexBindingDescriptions = vertices.bindingDescriptions.Data;
            vertices.inputState.vertexAttributeDescriptionCount = vertices.attributeDescriptions.Count;
            vertices.inputState.pVertexAttributeDescriptions = vertices.attributeDescriptions.Data;
        }

        void setupDescriptorPool()
        {
            NativeList<VkDescriptorPoolSize> poolSizes =
            {
            Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 1),
            Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, 1)
        };

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                Initializers.descriptorPoolCreateInfo(
                    poolSizes.Count,
                    poolSizes.Data,
                    2);

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, &descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            NativeList<VkDescriptorSetLayoutBinding> setLayoutBindings =
            {
            // Binding 0 : Vertex shader uniform buffer
            Initializers.descriptorSetLayoutBinding(
                VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                VK_SHADER_STAGE_VERTEX_BIT,
                0),
            // Binding 1 : Fragment shader image sampler (texture array)
            Initializers.descriptorSetLayoutBinding(
                VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                VK_SHADER_STAGE_FRAGMENT_BIT,
                1)
        };

            VkDescriptorSetLayoutCreateInfo descriptorLayout =
                Initializers.descriptorSetLayoutCreateInfo(
                    setLayoutBindings.Data,
                    setLayoutBindings.Count);

            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayout, null, &descriptorSetLayout));

            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo =
                Initializers.pipelineLayoutCreateInfo(
                    &descriptorSetLayout,
                    1);

            Util.CheckResult(vkCreatePipelineLayout(device, &pPipelineLayoutCreateInfo, null, &pipelineLayout));
        }

        void setupDescriptorSet()
        {
            VkDescriptorSetAllocateInfo allocInfo =
                Initializers.descriptorSetAllocateInfo(
                    descriptorPool,
                    &descriptorSetLayout,
                    1);

            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, &descriptorSet));

            // Image descriptor for the texture array
            VkDescriptorImageInfo textureDescriptor =
                Initializers.descriptorImageInfo(
                    textureArray.sampler,
                    textureArray.view,
                    textureArray.imageLayout);

            NativeList<VkWriteDescriptorSet> writeDescriptorSets =
            {
            // Binding 0 : Vertex shader uniform buffer
            Initializers.writeDescriptorSet(
                descriptorSet,
                VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                0,
                &uniformBufferVS.descriptor),
            // Binding 1 : Fragment shader cubemap sampler
            Initializers.writeDescriptorSet(
                descriptorSet,
                VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                1,
                &textureDescriptor)
        };

            vkUpdateDescriptorSets(device, writeDescriptorSets.Count, writeDescriptorSets.Data, 0, NULL);
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

            NativeList<VkDynamicState> dynamicStateEnables = {
            VK_DYNAMIC_STATE_VIEWPORT,
            VK_DYNAMIC_STATE_SCISSOR
        };
            VkPipelineDynamicStateCreateInfo dynamicState =
                Initializers.pipelineDynamicStateCreateInfo(
                    dynamicStateEnables.Data,
                    dynamicStateEnables.Count,
                    0);

            // Instacing pipeline
            // Load shaders
            std::array < VkPipelineShaderStageCreateInfo, 2 > shaderStages;

            shaderStages[0] = loadShader(getAssetPath() + "shaders/texturearray/instancing.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages[1] = loadShader(getAssetPath() + "shaders/texturearray/instancing.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);

            VkGraphicsPipelineCreateInfo pipelineCreateInfo =
                Initializers.pipelineCreateInfo(
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
            pipelineCreateInfo.stageCount = shaderStages.Count;
            pipelineCreateInfo.pStages = shaderStages.Data;

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, &pipeline));
        }

        void prepareUniformBuffers()
        {
            uboVS.instance = new UboInstanceData[layerCount];

            uint uboSize = sizeof(uboVS.matrices) + (layerCount * sizeof(UboInstanceData));

            // Vertex shader uniform buffer block
            Util.CheckResult(vulkanDevice->createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                &uniformBufferVS,
                uboSize));

            // Array indices and model matrices are fixed
            float offset = -1.5f;
            uint index = 0;
            float center = (layerCount * offset) / 2;
            for (int32_t i = 0; i < layerCount; i++)
            {
                // Instance model matrix
                uboVS.instance[i].model = glm::translate(Matrix4x4(), glm::vec3(0.0f, i * offset - center, 0.0f));
                uboVS.instance[i].model = glm::rotate(uboVS.instance[i].model, glm::radians(60.0f), glm::vec3(1.0f, 0.0f, 0.0f));
                // Instance texture array index
                uboVS.instance[i].arrayIndex.x = i;
            }

            // Update instanced part of the uniform buffer
            byte* pData;
            uint dataOffset = sizeof(uboVS.matrices);
            uint dataSize = layerCount * sizeof(UboInstanceData);
            Util.CheckResult(vkMapMemory(device, uniformBufferVS.memory, dataOffset, dataSize, 0, (void**)&pData));
            memcpy(pData, uboVS.instance, dataSize);
            vkUnmapMemory(device, uniformBufferVS.memory);

            // Map persistent
            Util.CheckResult(uniformBufferVS.map());

            updateUniformBufferMatrices();
        }

        void updateUniformBufferMatrices()
        {
            // Only updates the uniform buffer block part containing the global matrices

            // Projection
            uboVS.matrices.projection = glm::perspective(glm::radians(60.0f), (float)width / (float)height, 0.001f, 256.0f);

            // View
            uboVS.matrices.view = glm::translate(Matrix4x4(), glm::vec3(0.0f, -1.0f, zoom));
            uboVS.matrices.view = glm::rotate(uboVS.matrices.view, glm::radians(rotation.x), glm::vec3(1.0f, 0.0f, 0.0f));
            uboVS.matrices.view = glm::rotate(uboVS.matrices.view, glm::radians(rotation.y), glm::vec3(0.0f, 1.0f, 0.0f));
            uboVS.matrices.view = glm::rotate(uboVS.matrices.view, glm::radians(rotation.z), glm::vec3(0.0f, 0.0f, 1.0f));

            // Only update the matrices part of the uniform buffer
            memcpy(uniformBufferVS.mapped, &uboVS.matrices, sizeof(uboVS.matrices));
        }

        void draw()
        {
            VulkanExampleBase::prepareFrame();

            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = &drawCmdBuffers[currentBuffer];
            Util.CheckResult(vkQueueSubmit(queue, 1, &submitInfo, VK_NULL_HANDLE));

            VulkanExampleBase::submitFrame();
        }

        void prepare()
        {
            VulkanExampleBase::prepare();
            loadTextures();
            setupVertexDescriptions();
            generateQuad();
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
            updateUniformBufferMatrices();
        }

        public static void Main() => new TextureArrayExample().ExampleMain();
    }
}