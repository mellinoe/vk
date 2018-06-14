// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: pipelines/pipelines.cpp, 

/*
* Vulkan Example - Using different pipelines in one single renderpass
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class PipelinesExample : VulkanExampleBase, IDisposable
    {
        public const uint VERTEX_BUFFER_BIND_ID = 0;

        vksVertexLayout vertexLayout = GetVertexLayout();
        private static vksVertexLayout GetVertexLayout()
        {
            vksVertexLayout layout = new vksVertexLayout();
            layout.Components.Add(VertexComponent.VERTEX_COMPONENT_POSITION);
            layout.Components.Add(VertexComponent.VERTEX_COMPONENT_NORMAL);
            layout.Components.Add(VertexComponent.VERTEX_COMPONENT_UV);
            layout.Components.Add(VertexComponent.VERTEX_COMPONENT_COLOR);
            return layout;
        }

        vksModel models_cube = new vksModel();

        vksBuffer uniformBuffer = new vksBuffer();

        struct UboVS
        {
            public Matrix4x4 projection;
            public Matrix4x4 modelView;
            public Vector4 lightPos;

            public static UboVS CreateDefault() => new UboVS() { lightPos = new Vector4(0, 2, 1, 0) };
        }

        UboVS uboVS = UboVS.CreateDefault();

        VkPipelineLayout pipelineLayout;
        VkDescriptorSet descriptorSet;
        NativeList<VkDescriptorSetLayout> descriptorSetLayout = new NativeList<VkDescriptorSetLayout>();

        VkPipeline pipelines_phong;
        VkPipeline pipelines_wireframe;
        VkPipeline pipelines_toon;

        public PipelinesExample() : base()
        {
            zoom = -10.5f;
            rotation = new Vector3(-25.0f, 15.0f, 0.0f);
            title = "Vulkan Example - Pipeline state objects";
        }

        // C++ destructor
        public void Dispose()
        {
            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class
            vkDestroyPipeline(device, pipelines_phong, null);
            if (DeviceFeatures.fillModeNonSolid != 0)
            {
                vkDestroyPipeline(device, pipelines_wireframe, null);
            }
            vkDestroyPipeline(device, pipelines_toon, null);

            vkDestroyPipelineLayout(device, pipelineLayout, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayout[0], null);

            models_cube.destroy();
            uniformBuffer.destroy();
        }

        protected override void getEnabledFeatures()
        {
            // Enable physical Device features required for this example
            // Fill mode non solid is required for wireframe display
            if (DeviceFeatures.fillModeNonSolid != 0)
            {
                var features = enabledFeatures;
                features.fillModeNonSolid = True;
                // Wide lines must be present for line Width > 1.0f
                if (DeviceFeatures.wideLines != 0)
                {
                    features.wideLines = True;
                }

                enabledFeatures = features;
            }
        }

        protected override void buildCommandBuffers()
        {
            VkCommandBufferBeginInfo cmdBufInfo = VkCommandBufferBeginInfo.New();

            FixedArray2<VkClearValue> clearValues = new FixedArray2<VkClearValue>();
            clearValues.First.color = defaultClearColor;
            clearValues.Second.depthStencil = new VkClearDepthStencilValue() { depth = 1f, stencil = 0 };

            VkRenderPassBeginInfo renderPassBeginInfo = VkRenderPassBeginInfo.New();
            renderPassBeginInfo.renderPass = renderPass;
            renderPassBeginInfo.renderArea.offset.x = 0;
            renderPassBeginInfo.renderArea.offset.y = 0;
            renderPassBeginInfo.renderArea.extent.width = width;
            renderPassBeginInfo.renderArea.extent.height = height;
            renderPassBeginInfo.clearValueCount = 2;
            renderPassBeginInfo.pClearValues = (VkClearValue*)Unsafe.AsPointer(ref clearValues);

            for (int i = 0; i < drawCmdBuffers.Count; ++i)
            {
                // Set target frame buffer
                renderPassBeginInfo.framebuffer = frameBuffers[i];

                Util.CheckResult(vkBeginCommandBuffer(drawCmdBuffers[i], ref cmdBufInfo));

                vkCmdBeginRenderPass(drawCmdBuffers[i], ref renderPassBeginInfo, VkSubpassContents.Inline);

                VkViewport viewport = new VkViewport() { width = width, height = height, minDepth = 0f, maxDepth = 1f };
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, ref viewport);

                VkRect2D scissor = new VkRect2D() { extent = new VkExtent2D() { width = width, height = height }, offset = new VkOffset2D() };
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, ref scissor);

                vkCmdBindDescriptorSets(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelineLayout, 0, 1, ref descriptorSet, 0, null);

                ulong offsets = 0;
                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_cube.vertices.buffer, ref offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], models_cube.indices.buffer, 0, VkIndexType.Uint32);

                // Left : Solid colored 
                viewport.width = width / 3.0f;
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);
                vkCmdBindPipeline(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelines_phong);

                vkCmdDrawIndexed(drawCmdBuffers[i], models_cube.indexCount, 1, 0, 0, 0);

                // Center : Toon
                viewport.x = width / 3.0f;
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);
                vkCmdBindPipeline(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelines_toon);
                // Line Width > 1.0f only if wide lines feature is supported
                if (DeviceFeatures.wideLines != 0)
                {
                    vkCmdSetLineWidth(drawCmdBuffers[i], 2.0f);
                }
                vkCmdDrawIndexed(drawCmdBuffers[i], models_cube.indexCount, 1, 0, 0, 0);

                if (DeviceFeatures.fillModeNonSolid != 0)
                {
                    // Right : Wireframe 
                    viewport.x = width / 3.0f + width / 3.0f;
                    vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);
                    vkCmdBindPipeline(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelines_wireframe);
                    vkCmdDrawIndexed(drawCmdBuffers[i], models_cube.indexCount, 1, 0, 0, 0);
                }

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void loadAssets()
        {
            models_cube.loadFromFile(getAssetPath() + "models/treasure_smooth.dae", vertexLayout, 1.0f, vulkanDevice, queue);
        }

        void setupDescriptorPool()
        {
            VkDescriptorPoolSize poolSizes = new VkDescriptorPoolSize() { type = VkDescriptorType.UniformBuffer, descriptorCount = 1 };

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                VkDescriptorPoolCreateInfo.New();
            descriptorPoolInfo.poolSizeCount = 1;
            descriptorPoolInfo.pPoolSizes = &poolSizes;
            descriptorPoolInfo.maxSets = 2;

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, out descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            // Binding 0 : Vertex shader uniform buffer
            VkDescriptorSetLayoutBinding setLayoutBinding = new VkDescriptorSetLayoutBinding()
            {
                descriptorType = VkDescriptorType.UniformBuffer,
                stageFlags = VkShaderStageFlags.Vertex,
                binding = 0,
                descriptorCount = 1
            };

            VkDescriptorSetLayoutCreateInfo descriptorLayout = VkDescriptorSetLayoutCreateInfo.New();
            descriptorLayout.pBindings = &setLayoutBinding;
            descriptorLayout.bindingCount = 1;

            descriptorSetLayout.Count = 1;
            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayout, null, (VkDescriptorSetLayout*)descriptorSetLayout.GetAddress(0)));

            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo = VkPipelineLayoutCreateInfo.New();
            pPipelineLayoutCreateInfo.setLayoutCount = 1;
            pPipelineLayoutCreateInfo.pSetLayouts = (VkDescriptorSetLayout*)descriptorSetLayout.Data.ToPointer();

            Util.CheckResult(vkCreatePipelineLayout(device, &pPipelineLayoutCreateInfo, null, out pipelineLayout));
        }

        void setupDescriptorSet()
        {
            VkDescriptorSetAllocateInfo allocInfo = VkDescriptorSetAllocateInfo.New();
            allocInfo.descriptorPool = descriptorPool;
            allocInfo.pSetLayouts = (VkDescriptorSetLayout*)descriptorSetLayout.Data.ToPointer();
            allocInfo.descriptorSetCount = 1;
            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSet));

            var descriptor = uniformBuffer.descriptor;
            VkWriteDescriptorSet writeDescriptorSets = Initializers.writeDescriptorSet(
                descriptorSet,
                 VkDescriptorType.UniformBuffer,
                0,
                &descriptor);
            vkUpdateDescriptorSets(device, 1, ref writeDescriptorSets, 0, null);
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
                    VkFrontFace.Clockwise,
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
                    True,
                    True,
                    VkCompareOp.LessOrEqual);

            VkPipelineViewportStateCreateInfo viewportState =
                Initializers.pipelineViewportStateCreateInfo(1, 1, 0);

            VkPipelineMultisampleStateCreateInfo multisampleState =
                Initializers.pipelineMultisampleStateCreateInfo(VkSampleCountFlags.Count1);

            FixedArray3<VkDynamicState> dynamicStateEnables = new FixedArray3<VkDynamicState>(
                 VkDynamicState.Viewport,
                 VkDynamicState.Scissor,
                 VkDynamicState.LineWidth);
            VkPipelineDynamicStateCreateInfo dynamicState =
                Initializers.pipelineDynamicStateCreateInfo((VkDynamicState*)Unsafe.AsPointer(ref dynamicStateEnables), dynamicStateEnables.Count);

            VkGraphicsPipelineCreateInfo pipelineCreateInfo =
                Initializers.pipelineCreateInfo(pipelineLayout, renderPass);

            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStages = new FixedArray2<VkPipelineShaderStageCreateInfo>();
            shaderStages.First = loadShader(getAssetPath() + "shaders/pipelines/phong.vert.spv", VkShaderStageFlags.Vertex);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/pipelines/phong.frag.spv", VkShaderStageFlags.Fragment);

            pipelineCreateInfo.pInputAssemblyState = &inputAssemblyState;
            pipelineCreateInfo.pRasterizationState = &rasterizationState;
            pipelineCreateInfo.pColorBlendState = &colorBlendState;
            pipelineCreateInfo.pMultisampleState = &multisampleState;
            pipelineCreateInfo.pViewportState = &viewportState;
            pipelineCreateInfo.pDepthStencilState = &depthStencilState;
            pipelineCreateInfo.pDynamicState = &dynamicState;
            pipelineCreateInfo.stageCount = shaderStages.Count;
            pipelineCreateInfo.pStages = (VkPipelineShaderStageCreateInfo*)Unsafe.AsPointer(ref shaderStages);

            // Shared vertex bindings and attributes used by all pipelines

            // Binding description
            VkVertexInputBindingDescription vertexInputBindings
                = Initializers.vertexInputBindingDescription(VERTEX_BUFFER_BIND_ID, vertexLayout.GetStride(), VkVertexInputRate.Vertex);

            // Attribute descriptions
            FixedArray4<VkVertexInputAttributeDescription> vertexInputAttributes = new FixedArray4<VkVertexInputAttributeDescription>
            {
                First = Initializers.vertexInputAttributeDescription(VERTEX_BUFFER_BIND_ID, 0, VkFormat.R32g32b32Sfloat, 0),                   // Location 0: Position            
                Second = Initializers.vertexInputAttributeDescription(VERTEX_BUFFER_BIND_ID, 1, VkFormat.R32g32b32Sfloat, sizeof(float) * 3),    // Location 1: Color            
                Third = Initializers.vertexInputAttributeDescription(VERTEX_BUFFER_BIND_ID, 2, VkFormat.R32g32Sfloat, sizeof(float) * 6),       // Location 2 : Texture coordinates            
                Fourth = Initializers.vertexInputAttributeDescription(VERTEX_BUFFER_BIND_ID, 3, VkFormat.R32g32b32Sfloat, sizeof(float) * 8),    // Location 3 : Normal
            };

            VkPipelineVertexInputStateCreateInfo vertexInputState = VkPipelineVertexInputStateCreateInfo.New();
            vertexInputState.vertexBindingDescriptionCount = 1;
            vertexInputState.pVertexBindingDescriptions = &vertexInputBindings;
            vertexInputState.vertexAttributeDescriptionCount = vertexInputAttributes.Count;
            vertexInputState.pVertexAttributeDescriptions = (VkVertexInputAttributeDescription*)Unsafe.AsPointer(ref vertexInputAttributes);

            pipelineCreateInfo.pVertexInputState = &vertexInputState;

            // Create the graphics pipeline state objects

            // We are using this pipeline as the base for the other pipelines (derivatives)
            // Pipeline derivatives can be used for pipelines that share most of their state
            // Depending on the implementation this may result in better performance for pipeline 
            // switchting and faster creation time
            pipelineCreateInfo.flags = VkPipelineCreateFlags.AllowDerivatives;

            // Textured pipeline
            // Phong shading pipeline
            shaderStages.First = loadShader(getAssetPath() + "shaders/pipelines/phong.vert.spv", VkShaderStageFlags.Vertex);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/pipelines/phong.frag.spv", VkShaderStageFlags.Fragment);
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, ref pipelineCreateInfo, null, out pipelines_phong));

            // All pipelines created after the base pipeline will be derivatives
            pipelineCreateInfo.flags = VkPipelineCreateFlags.Derivative;
            // Base pipeline will be our first created pipeline
            pipelineCreateInfo.basePipelineHandle = pipelines_phong;
            // It's only allowed to either use a handle or index for the base pipeline
            // As we use the handle, we must set the index to -1 (see section 9.5 of the specification)
            pipelineCreateInfo.basePipelineIndex = -1;

            // Toon shading pipeline
            shaderStages.First = loadShader(getAssetPath() + "shaders/pipelines/toon.vert.spv", VkShaderStageFlags.Vertex);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/pipelines/toon.frag.spv", VkShaderStageFlags.Fragment);
            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, ref pipelineCreateInfo, null, out pipelines_toon));

            // Pipeline for wire frame rendering
            // Non solid rendering is not a mandatory Vulkan feature
            if (DeviceFeatures.fillModeNonSolid != 0)
            {
                rasterizationState.polygonMode = VkPolygonMode.Line;
                shaderStages.First = loadShader(getAssetPath() + "shaders/pipelines/wireframe.vert.spv", VkShaderStageFlags.Vertex);
                shaderStages.Second = loadShader(getAssetPath() + "shaders/pipelines/wireframe.frag.spv", VkShaderStageFlags.Fragment);
                Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, ref pipelineCreateInfo, null, out pipelines_wireframe));
            }
        }

        // Prepare and initialize uniform buffer containing shader uniforms
        void prepareUniformBuffers()
        {
            // Create the vertex shader uniform buffer block
            Util.CheckResult(vulkanDevice.createBuffer(
                VkBufferUsageFlags.UniformBuffer,
                VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent,
                uniformBuffer,
                (ulong)sizeof(UboVS)));

            // Map persistent
            Util.CheckResult(uniformBuffer.map());

            updateUniformBuffers();
        }

        void updateUniformBuffers()
        {
            uboVS.projection = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(60f), width / 3.0f / (float)height, 0.1f, 256.0f);

            Matrix4x4 viewMatrix = Matrix4x4.CreateTranslation(new Vector3(0f, 0f, zoom));

            uboVS.modelView = viewMatrix * Matrix4x4.CreateTranslation(cameraPos);

            uboVS.modelView = Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, Util.DegreesToRadians(rotation.X)) * uboVS.modelView;
            uboVS.modelView = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, Util.DegreesToRadians(rotation.Y)) * uboVS.modelView;
            uboVS.modelView = Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, Util.DegreesToRadians(rotation.Z)) * uboVS.modelView;

            UboVS uboVSLocal = uboVS;
            Unsafe.CopyBlock(uniformBuffer.mapped, &uboVSLocal, (uint)sizeof(UboVS));
        }

        void draw()
        {
            base.prepareFrame();

            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = (VkCommandBuffer*)drawCmdBuffers.GetAddress(currentBuffer);
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, new VkFence()));

            base.submitFrame();
        }
        public override void Prepare()
        {
            base.Prepare();
            loadAssets();
            prepareUniformBuffers();
            setupDescriptorSetLayout();
            preparePipelines();
            setupDescriptorPool();
            setupDescriptorSet();
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

        public static void Main() => new PipelinesExample().ExampleMain();
    }
}
