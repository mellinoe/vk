// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: texturecubemap/texturecubemap.cpp, 

/*
* Vulkan Example - Cube map texture loading and displaying
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;
using Veldrid.Sdl2;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class TextureCubemapExample : VulkanExampleBase
    {
        public bool displaySkybox = true;
        public vksTexture cubeMap = new vksTexture();

        public class Vertices
        {
            public VkPipelineVertexInputStateCreateInfo inputState;
            public NativeList<VkVertexInputBindingDescription> bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
            public NativeList<VkVertexInputAttributeDescription> attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();
        }

        public Vertices vertices = new Vertices();

        // Vertex layout for the models
        public vksVertexLayout vertexLayout = new vksVertexLayout(
            VertexComponent.VERTEX_COMPONENT_POSITION,
            VertexComponent.VERTEX_COMPONENT_NORMAL,
            VertexComponent.VERTEX_COMPONENT_UV);

        public vksModel models_skybox = new vksModel();
        public List<vksModel> models_objects = new List<vksModel>();
        public uint models_objectIndex = 0;

        public vksBuffer uniformBuffers_object = new vksBuffer();
        public vksBuffer uniformBuffers_skybox = new vksBuffer();

        public struct UboVS
        {
            public Matrix4x4 projection;
            public Matrix4x4 model;
            public float lodBias;
        }

        public UboVS uboVS;

        public VkPipeline pipelines_skybox;
        public VkPipeline pipelines_reflect;

        public VkDescriptorSet descriptorSets_object;
        public VkDescriptorSet descriptorSets_skybox;

        public VkPipelineLayout pipelineLayout;
        public VkDescriptorSetLayout descriptorSetLayout;
        private const uint VERTEX_BUFFER_BIND_ID = 0;

        TextureCubemapExample()
        {
            zoom = -4.0f;
            rotationSpeed = 0.25f;
            rotation = new Vector3(-7.25f, -120.0f, 0.0f);
            // enableTextOverlay = true;
            title = "Vulkan Example - Cube map";
        }

        public void Dispose()
        {
            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class

            // Clean up texture resources
            vkDestroyImageView(device, cubeMap.view, null);
            vkDestroyImage(device, cubeMap.image, null);
            vkDestroySampler(device, cubeMap.sampler, null);
            vkFreeMemory(device, cubeMap.deviceMemory, null);

            vkDestroyPipeline(device, pipelines_skybox, null);
            vkDestroyPipeline(device, pipelines_reflect, null);

            vkDestroyPipelineLayout(device, pipelineLayout, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayout, null);

            foreach (var model in models_objects)
            {
                model.destroy();
            }
            models_skybox.destroy();

            uniformBuffers_object.destroy();
            uniformBuffers_skybox.destroy();
        }

        void loadCubemap(string filename, VkFormat format, bool forceLinearTiling)
        {
            KtxFile texCube;
            using (var fs = File.OpenRead(filename))
            {
                texCube = KtxFile.Load(fs, readKeyValuePairs: false);
            }

            cubeMap.width = texCube.Header.PixelWidth;
            cubeMap.height = texCube.Header.PixelHeight;
            cubeMap.mipLevels = texCube.Header.NumberOfMipmapLevels;

            VkMemoryAllocateInfo memAllocInfo = Initializers.memoryAllocateInfo();
            VkMemoryRequirements memReqs;

            // Create a host-visible staging buffer that contains the raw image data
            VkBuffer stagingBuffer;
            VkDeviceMemory stagingMemory;

            VkBufferCreateInfo bufferCreateInfo = Initializers.bufferCreateInfo();
            bufferCreateInfo.size = texCube.GetTotalSize();
            // This buffer is used as a transfer source for the buffer copy
            bufferCreateInfo.usage = VkBufferUsageFlags.TransferSrc;
            bufferCreateInfo.sharingMode = VkSharingMode.Exclusive;

            Util.CheckResult(vkCreateBuffer(device, &bufferCreateInfo, null, &stagingBuffer));

            // Get memory requirements for the staging buffer (alignment, memory type bits)
            vkGetBufferMemoryRequirements(device, stagingBuffer, &memReqs);
            memAllocInfo.allocationSize = memReqs.size;
            // Get memory type index for a host visible buffer
            memAllocInfo.memoryTypeIndex = vulkanDevice.getMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);
            Util.CheckResult(vkAllocateMemory(device, &memAllocInfo, null, &stagingMemory));
            Util.CheckResult(vkBindBufferMemory(device, stagingBuffer, stagingMemory, 0));

            // Copy texture data into staging buffer
            byte* data;
            Util.CheckResult(vkMapMemory(device, stagingMemory, 0, memReqs.size, 0, (void**)&data));
            byte[] allTextureData = texCube.GetAllTextureData();
            fixed (byte* texCubeDataPtr = &allTextureData[0])
            {
                Unsafe.CopyBlock(data, texCubeDataPtr, (uint)allTextureData.Length);
            }

            vkUnmapMemory(device, stagingMemory);

            // Create optimal tiled target image
            VkImageCreateInfo imageCreateInfo = Initializers.imageCreateInfo();
            imageCreateInfo.imageType = VkImageType.Image2D;
            imageCreateInfo.format = format;
            imageCreateInfo.mipLevels = cubeMap.mipLevels;
            imageCreateInfo.samples = VkSampleCountFlags.Count1;
            imageCreateInfo.tiling = VkImageTiling.Optimal;
            imageCreateInfo.usage = VkImageUsageFlags.Sampled;
            imageCreateInfo.sharingMode = VkSharingMode.Exclusive;
            imageCreateInfo.initialLayout = VkImageLayout.Undefined;
            imageCreateInfo.extent = new VkExtent3D { width = cubeMap.width, height = cubeMap.height, depth = 1 };
            imageCreateInfo.usage = VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;
            // Cube faces count as array layers in Vulkan
            imageCreateInfo.arrayLayers = 6;
            // This flag is required for cube map images
            imageCreateInfo.flags = VkImageCreateFlags.CubeCompatible;

            Util.CheckResult(vkCreateImage(device, &imageCreateInfo, null, out cubeMap.image));

            vkGetImageMemoryRequirements(device, cubeMap.image, &memReqs);

            memAllocInfo.allocationSize = memReqs.size;
            memAllocInfo.memoryTypeIndex = vulkanDevice.getMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

            Util.CheckResult(vkAllocateMemory(device, &memAllocInfo, null, out cubeMap.deviceMemory));
            Util.CheckResult(vkBindImageMemory(device, cubeMap.image, cubeMap.deviceMemory, 0));

            VkCommandBuffer copyCmd = createCommandBuffer(VkCommandBufferLevel.Primary, true);

            // Setup buffer copy regions for each face including all of it's miplevels
            NativeList<VkBufferImageCopy> bufferCopyRegions = new NativeList<VkBufferImageCopy>();
            uint offset = 0;

            for (uint face = 0; face < 6; face++)
            {
                for (uint level = 0; level < cubeMap.mipLevels; level++)
                {
                    VkBufferImageCopy bufferCopyRegion = new VkBufferImageCopy();
                    bufferCopyRegion.imageSubresource.aspectMask = VkImageAspectFlags.Color;
                    bufferCopyRegion.imageSubresource.mipLevel = level;
                    bufferCopyRegion.imageSubresource.baseArrayLayer = face;
                    bufferCopyRegion.imageSubresource.layerCount = 1;
                    bufferCopyRegion.imageExtent.width = texCube.Faces[face].Mipmaps[level].Width;
                    bufferCopyRegion.imageExtent.height = texCube.Faces[face].Mipmaps[level].Height;
                    bufferCopyRegion.imageExtent.depth = 1;
                    bufferCopyRegion.bufferOffset = offset;

                    bufferCopyRegions.Add(bufferCopyRegion);

                    // Increase offset into staging buffer for next level / face
                    offset += texCube.Faces[face].Mipmaps[level].SizeInBytes;
                }
            }

            // Image barrier for optimal image (target)
            // Set initial layout for all array layers (faces) of the optimal (target) tiled texture
            VkImageSubresourceRange subresourceRange = new VkImageSubresourceRange();
            subresourceRange.aspectMask = VkImageAspectFlags.Color;
            subresourceRange.baseMipLevel = 0;
            subresourceRange.levelCount = cubeMap.mipLevels;
            subresourceRange.layerCount = 6;

            Tools.setImageLayout(
                copyCmd,
                cubeMap.image,
                VkImageAspectFlags.Color,
                VkImageLayout.Undefined,
                VkImageLayout.TransferDstOptimal,
                subresourceRange);

            // Copy the cube map faces from the staging buffer to the optimal tiled image
            vkCmdCopyBufferToImage(
                copyCmd,
                stagingBuffer,
                cubeMap.image,
                VkImageLayout.TransferDstOptimal,
                bufferCopyRegions.Count,
                bufferCopyRegions.Data);

            // Change texture image layout to shader read after all faces have been copied
            cubeMap.imageLayout = VkImageLayout.ShaderReadOnlyOptimal;
            Tools.setImageLayout(
                copyCmd,
                cubeMap.image,
                VkImageAspectFlags.Color,
                VkImageLayout.TransferDstOptimal,
                cubeMap.imageLayout,
                subresourceRange);

            flushCommandBuffer(copyCmd, queue, true);

            // Create sampler
            VkSamplerCreateInfo sampler = Initializers.samplerCreateInfo();
            sampler.magFilter = VkFilter.Linear;
            sampler.minFilter = VkFilter.Linear;
            sampler.mipmapMode = VkSamplerMipmapMode.Linear;
            sampler.addressModeU = VkSamplerAddressMode.ClampToEdge;
            sampler.addressModeV = sampler.addressModeU;
            sampler.addressModeW = sampler.addressModeU;
            sampler.mipLodBias = 0.0f;
            sampler.compareOp = VkCompareOp.Never;
            sampler.minLod = 0.0f;
            sampler.maxLod = cubeMap.mipLevels;
            sampler.borderColor = VkBorderColor.FloatOpaqueWhite;
            sampler.maxAnisotropy = 1.0f;
            if (vulkanDevice.features.samplerAnisotropy == 1)
            {
                sampler.maxAnisotropy = vulkanDevice.properties.limits.maxSamplerAnisotropy;
                sampler.anisotropyEnable = True;
            }
            Util.CheckResult(vkCreateSampler(device, &sampler, null, out cubeMap.sampler));

            // Create image view
            VkImageViewCreateInfo view = Initializers.imageViewCreateInfo();
            // Cube map view type
            view.viewType = VkImageViewType.ImageCube;
            view.format = format;
            view.components = new VkComponentMapping { r = VkComponentSwizzle.R, g = VkComponentSwizzle.G, b = VkComponentSwizzle.B, a = VkComponentSwizzle.A };
            view.subresourceRange = new VkImageSubresourceRange { aspectMask = VkImageAspectFlags.Color, baseMipLevel = 0, layerCount = 1, baseArrayLayer = 0, levelCount = 1 };
            // 6 array layers (faces)
            view.subresourceRange.layerCount = 6;
            // Set number of mip levels
            view.subresourceRange.levelCount = cubeMap.mipLevels;
            view.image = cubeMap.image;
            Util.CheckResult(vkCreateImageView(device, &view, null, out cubeMap.view));

            // Clean up staging resources
            vkFreeMemory(device, stagingMemory, null);
            vkDestroyBuffer(device, stagingBuffer, null);
        }

        void loadTextures()
        {
            // Vulkan core supports three different compressed texture formats
            // As the support differs between implemementations we need to check Device features and select a proper format and file
            string filename;
            VkFormat format;
            if (DeviceFeatures.textureCompressionBC == 1)
            {
                filename = "cubemap_yokohama_bc3_unorm.ktx";
                format = VkFormat.Bc2UnormBlock;
            }
            else if (DeviceFeatures.textureCompressionASTC_LDR == 1)
            {
                filename = "cubemap_yokohama_astc_8x8_unorm.ktx";
                format = VkFormat.Astc8x8UnormBlock;
            }
            else if (DeviceFeatures.textureCompressionETC2 == 1)
            {
                filename = "cubemap_yokohama_etc2_unorm.ktx";
                format = VkFormat.Etc2R8g8b8UnormBlock;
            }
            else
            {
                throw new InvalidOperationException("Device does not support any compressed texture format!");
            }

            loadCubemap(getAssetPath() + "textures/" + filename, format, false);
        }


        void ReBuildCommandBuffers()
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

                vkCmdBeginRenderPass(drawCmdBuffers[i], &renderPassBeginInfo, VkSubpassContents.Inline);

                VkViewport viewport = Initializers.viewport((float)width, (float)height, 0.0f, 1.0f);
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);

                VkRect2D scissor = Initializers.rect2D(width, height, 0, 0);
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, &scissor);

                ulong offsets = 0;

                // Skybox
                if (displaySkybox)
                {
                    vkCmdBindDescriptorSets(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelineLayout, 0, 1, ref descriptorSets_skybox, 0, null);
                    vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_skybox.vertices.buffer, ref offsets);
                    vkCmdBindIndexBuffer(drawCmdBuffers[i], models_skybox.indices.buffer, 0, VkIndexType.Uint32);
                    vkCmdBindPipeline(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelines_skybox);
                    vkCmdDrawIndexed(drawCmdBuffers[i], models_skybox.indexCount, 1, 0, 0, 0);
                }

                // 3D object
                vkCmdBindDescriptorSets(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelineLayout, 0, 1, ref descriptorSets_object, 0, null);
                var vbuffer = models_objects[(int)models_objectIndex].vertices.buffer;
                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, &vbuffer, ref offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], models_objects[(int)models_objectIndex].indices.buffer, 0, VkIndexType.Uint32);
                vkCmdBindPipeline(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelines_reflect);
                vkCmdDrawIndexed(drawCmdBuffers[i], models_objects[(int)models_objectIndex].indexCount, 1, 0, 0, 0);

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void loadMeshes()
        {
            // Skybox
            models_skybox.loadFromFile(getAssetPath() + "models/cube.obj", vertexLayout, 0.05f, vulkanDevice, queue);
            // Objects
            List<string> filenames = new List<string> { "sphere.obj", "teapot.dae", "torusknot.obj" };
            foreach (string file in filenames)
            {
                vksModel model = new vksModel();
                model.loadFromFile(getAssetPath() + "models/" + file, vertexLayout, 0.05f, vulkanDevice, queue);
                models_objects.Add(model);
            }
        }

        void setupVertexDescriptions()
        {
            // Binding description
            vertices.bindingDescriptions.Count = 1;
            vertices.bindingDescriptions[0] =
                Initializers.vertexInputBindingDescription(
                    VERTEX_BUFFER_BIND_ID,
                    vertexLayout.GetStride(),
                    VkVertexInputRate.Vertex);

            // Attribute descriptions
            // Describes memory layout and shader positions
            vertices.attributeDescriptions.Count = 3;
            // Location 0 : Position
            vertices.attributeDescriptions[0] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    0,
                    VkFormat.R32g32b32Sfloat,
                    0);
            // Location 1 : Normal
            vertices.attributeDescriptions[1] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    1,
                    VkFormat.R32g32b32Sfloat,
                    sizeof(float) * 3);
            // Location 2 : Texture coordinates
            vertices.attributeDescriptions[2] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    2,
                    VkFormat.R32g32Sfloat,
                    sizeof(float) * 5);

            vertices.inputState = Initializers.pipelineVertexInputStateCreateInfo();
            vertices.inputState.vertexBindingDescriptionCount = vertices.bindingDescriptions.Count;
            vertices.inputState.pVertexBindingDescriptions = (VkVertexInputBindingDescription*)vertices.bindingDescriptions.Data;
            vertices.inputState.vertexAttributeDescriptionCount = vertices.attributeDescriptions.Count;
            vertices.inputState.pVertexAttributeDescriptions = (VkVertexInputAttributeDescription*)vertices.attributeDescriptions.Data;
        }

        void setupDescriptorPool()
        {
            FixedArray2<VkDescriptorPoolSize> poolSizes = new FixedArray2<VkDescriptorPoolSize>(
                Initializers.descriptorPoolSize(VkDescriptorType.UniformBuffer, 2),
                Initializers.descriptorPoolSize(VkDescriptorType.CombinedImageSampler, 2));

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                Initializers.descriptorPoolCreateInfo(
                    poolSizes.Count,
                    &poolSizes.First,
                    2);

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, out descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            FixedArray2<VkDescriptorSetLayoutBinding> setLayoutBindings = new FixedArray2<VkDescriptorSetLayoutBinding>(
                // Binding 0 : Vertex shader uniform buffer
                Initializers.descriptorSetLayoutBinding(
                    VkDescriptorType.UniformBuffer,
                    VkShaderStageFlags.Vertex,
                    0),
                // Binding 1 : Fragment shader image sampler
                Initializers.descriptorSetLayoutBinding(
                    VkDescriptorType.CombinedImageSampler,
                    VkShaderStageFlags.Fragment,
                    1));

            VkDescriptorSetLayoutCreateInfo descriptorLayout =
                Initializers.descriptorSetLayoutCreateInfo(
                    &setLayoutBindings.First,
                    setLayoutBindings.Count);

            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayout, null, out descriptorSetLayout));

            var dsl = descriptorSetLayout;
            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo =
                Initializers.pipelineLayoutCreateInfo(
                    &dsl,
                    1);

            Util.CheckResult(vkCreatePipelineLayout(device, &pPipelineLayoutCreateInfo, null, out pipelineLayout));
        }

        void setupDescriptorSets()
        {
            // Image descriptor for the cube map texture
            VkDescriptorImageInfo textureDescriptor =
                Initializers.descriptorImageInfo(
                    cubeMap.sampler,
                    cubeMap.view,
                    cubeMap.imageLayout);

            var dsl = descriptorSetLayout;
            VkDescriptorSetAllocateInfo allocInfo =
                Initializers.descriptorSetAllocateInfo(
                    descriptorPool,
                    &dsl,
                    1);

            // 3D object descriptor set
            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSets_object));

            var bufferInfo = uniformBuffers_object.descriptor;
            FixedArray2<VkWriteDescriptorSet> writeDescriptorSets = new FixedArray2<VkWriteDescriptorSet>
            (
                // Binding 0 : Vertex shader uniform buffer
                Initializers.writeDescriptorSet(
                    descriptorSets_object,
                    VkDescriptorType.UniformBuffer,
                    0,
                    &bufferInfo),
                // Binding 1 : Fragment shader cubemap sampler
                Initializers.writeDescriptorSet(
                    descriptorSets_object,
                     VkDescriptorType.CombinedImageSampler,
                    1,
                    &textureDescriptor));

            vkUpdateDescriptorSets(device, writeDescriptorSets.Count, ref writeDescriptorSets.First, 0, null);

            // Sky box descriptor set
            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSets_skybox));

            var descriptor = uniformBuffers_skybox.descriptor;
            writeDescriptorSets = new FixedArray2<VkWriteDescriptorSet>
            (
                // Binding 0 : Vertex shader uniform buffer
                Initializers.writeDescriptorSet(
                    descriptorSets_skybox,
                    VkDescriptorType.UniformBuffer,
                    0,
                    &descriptor),
                // Binding 1 : Fragment shader cubemap sampler
                Initializers.writeDescriptorSet(
                    descriptorSets_skybox,
                    VkDescriptorType.CombinedImageSampler,
                    1,
                    &textureDescriptor));

            vkUpdateDescriptorSets(device, writeDescriptorSets.Count, ref writeDescriptorSets.First, 0, null);
        }

        void preparePipelines()
        {
            VkPipelineInputAssemblyStateCreateInfo inputAssemblyState =
                Initializers.pipelineInputAssemblyStateCreateInfo(
                    VkPrimitiveTopology.TriangleList,
                    0,
                    False);

            VkPipelineRasterizationStateCreateInfo rasterizationState =
                Initializers.pipelineRasterizationStateCreateInfo(
                    VkPolygonMode.Fill,
                    VkCullModeFlags.Back,
                    VkFrontFace.CounterClockwise,
                    0);

            VkPipelineColorBlendAttachmentState blendAttachmentState =
                Initializers.pipelineColorBlendAttachmentState(
                    (VkColorComponentFlags)0xf,
                    False);

            VkPipelineColorBlendStateCreateInfo colorBlendState =
                Initializers.pipelineColorBlendStateCreateInfo(
                    1,
                    &blendAttachmentState);

            VkPipelineDepthStencilStateCreateInfo depthStencilState =
                Initializers.pipelineDepthStencilStateCreateInfo(
                    False,
                    False,
                    VkCompareOp.LessOrEqual);

            VkPipelineViewportStateCreateInfo viewportState =
                Initializers.pipelineViewportStateCreateInfo(1, 1, 0);

            VkPipelineMultisampleStateCreateInfo multisampleState =
                Initializers.pipelineMultisampleStateCreateInfo(
                    VkSampleCountFlags.Count1,
                    0);

            FixedArray2<VkDynamicState> dynamicStateEnables = new FixedArray2<VkDynamicState>(
                 VkDynamicState.Viewport,
                 VkDynamicState.Scissor);
            VkPipelineDynamicStateCreateInfo dynamicState =
                Initializers.pipelineDynamicStateCreateInfo(
                    &dynamicStateEnables.First,
                    dynamicStateEnables.Count,
                    0);

            // Skybox pipeline (background cube)
            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStages = new FixedArray2<VkPipelineShaderStageCreateInfo>(
                loadShader(getAssetPath() + "shaders/cubemap/skybox.vert.spv", VkShaderStageFlags.Vertex),
                loadShader(getAssetPath() + "shaders/cubemap/skybox.frag.spv", VkShaderStageFlags.Fragment));

            VkGraphicsPipelineCreateInfo pipelineCreateInfo =
                Initializers.pipelineCreateInfo(
                    pipelineLayout,
                    renderPass,
                    0);

            var inputState = vertices.inputState;
            pipelineCreateInfo.pVertexInputState = &inputState;
            pipelineCreateInfo.pInputAssemblyState = &inputAssemblyState;
            pipelineCreateInfo.pRasterizationState = &rasterizationState;
            pipelineCreateInfo.pColorBlendState = &colorBlendState;
            pipelineCreateInfo.pMultisampleState = &multisampleState;
            pipelineCreateInfo.pViewportState = &viewportState;
            pipelineCreateInfo.pDepthStencilState = &depthStencilState;
            pipelineCreateInfo.pDynamicState = &dynamicState;
            pipelineCreateInfo.stageCount = shaderStages.Count;
            pipelineCreateInfo.pStages = &shaderStages.First;

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_skybox));

            // Cube map reflect pipeline
            shaderStages.First = loadShader(getAssetPath() + "shaders/cubemap/reflect.vert.spv", VkShaderStageFlags.Vertex);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/cubemap/reflect.frag.spv", VkShaderStageFlags.Fragment);
            // Enable depth test and write
            depthStencilState.depthWriteEnable = True;
            depthStencilState.depthTestEnable = True;
            // Flip cull mode
            rasterizationState.cullMode = VkCullModeFlags.Front;
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_reflect));
        }

        // Prepare and initialize uniform buffer containing shader uniforms
        void prepareUniformBuffers()
        {
            // Objact vertex shader uniform buffer
            Util.CheckResult(vulkanDevice.createBuffer(
                VkBufferUsageFlags.UniformBuffer,
                VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent,
                uniformBuffers_object,
                (ulong)sizeof(UboVS)));

            // Skybox vertex shader uniform buffer
            Util.CheckResult(vulkanDevice.createBuffer(
                VkBufferUsageFlags.UniformBuffer,
                VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent,
                uniformBuffers_skybox,
                (uint)sizeof(UboVS)));

            // Map persistent
            Util.CheckResult(uniformBuffers_object.map());
            Util.CheckResult(uniformBuffers_skybox.map());

            updateUniformBuffers();
        }

        void updateUniformBuffers()
        {
            // 3D object
            uboVS.projection = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(60f), width / (float)height, 0.001f, 256.0f);
            Matrix4x4 viewMatrix = Matrix4x4.CreateTranslation(new Vector3(0, 0, zoom));

            uboVS.model = viewMatrix * Matrix4x4.CreateTranslation(cameraPos);
            uboVS.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * uboVS.model;
            uboVS.model = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y)) * uboVS.model;
            uboVS.model = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * uboVS.model;

            var local = uboVS;
            Unsafe.CopyBlock(uniformBuffers_object.mapped, &local, (uint)sizeof(UboVS));

            // Skybox
            viewMatrix = Matrix4x4.Identity;
            uboVS.projection = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(60f), width / (float)height, 0.001f, 256.0f);

            uboVS.model = Matrix4x4.Identity;
            uboVS.model = viewMatrix;
            uboVS.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * uboVS.model;
            uboVS.model = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y)) * uboVS.model;
            uboVS.model = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * uboVS.model;

            local = uboVS;
            Unsafe.CopyBlock(uniformBuffers_skybox.mapped, &local, (uint)sizeof(UboVS));
        }

        void draw()
        {
            prepareFrame();

            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = (VkCommandBuffer*)drawCmdBuffers.GetAddress(currentBuffer);
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, VkFence.Null));

            submitFrame();
        }

        public override void Prepare()
        {
            base.Prepare();
            loadTextures();
            loadMeshes();
            setupVertexDescriptions();
            prepareUniformBuffers();
            setupDescriptorSetLayout();
            preparePipelines();
            setupDescriptorPool();
            setupDescriptorSets();
            buildCommandBuffers();
            prepared = true;
        }

        protected override void render()
        {
            if (!prepared)
                return;
            draw();
        }

        protected override void viewChanged()
        {
            updateUniformBuffers();
        }

        void toggleSkyBox()
        {
            displaySkybox = !displaySkybox;
            ReBuildCommandBuffers();
        }

        void toggleObject()
        {
            models_objectIndex++;
            if (models_objectIndex >= models_objects.Count)
            {
                models_objectIndex = 0;
            }
            ReBuildCommandBuffers();
        }

        void changeLodBias(float delta)
        {
            uboVS.lodBias += delta;
            if (uboVS.lodBias < 0.0f)
            {
                uboVS.lodBias = 0.0f;
            }
            if (uboVS.lodBias > cubeMap.mipLevels)
            {
                uboVS.lodBias = cubeMap.mipLevels;
            }
            updateUniformBuffers();
            //updateTextOverlay();
        }

        protected override void keyPressed(Key keyCode)
        {
            switch (keyCode)
            {
                case Key.S:
                    toggleSkyBox();
                    break;
                case Key.Space:
                    toggleObject();
                    break;
                case Key.KeypadAdd:
                    changeLodBias(0.1f);
                    break;
                case Key.KeypadSubtract:
                    changeLodBias(-0.1f);
                    break;
            }
        }

        public static void Main() => new TextureCubemapExample().ExampleMain();
    }
}
