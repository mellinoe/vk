// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: pushconstants/pushconstants.cpp, 

/*
* Vulkan Example - Push constants example (small shader block accessed outside of uniforms for fast updates)
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vulkan;
using System.Diagnostics;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class PushConstantsExample : VulkanExampleBase
    {
        private const uint VERTEX_BUFFER_BIND_ID = 0;
        private VkPipelineVertexInputStateCreateInfo vertices_inputState;
        private NativeList<VkVertexInputBindingDescription> vertices_bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
        private NativeList<VkVertexInputAttributeDescription> vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();

        // Vertex layout for the models
        private vksVertexLayout vertexLayout = new vksVertexLayout(
            VertexComponent.VERTEX_COMPONENT_POSITION,
            VertexComponent.VERTEX_COMPONENT_NORMAL,
            VertexComponent.VERTEX_COMPONENT_UV,
            VertexComponent.VERTEX_COMPONENT_COLOR);

        private vksModel models_scene = new vksModel();

        private vksBuffer uniformBuffer = new vksBuffer();

        public struct UboVS
        {
            public Matrix4x4 projection;
            public Matrix4x4 model;
            public Vector4 lightPos;
        }

        private UboVS _uboVS = new UboVS() { lightPos = new Vector4(0, 0, -2, 1) };

        private VkPipeline pipelines_solid;

        VkPipelineLayout pipelineLayout;
        VkDescriptorSet descriptorSet;
        VkDescriptorSetLayout descriptorSetLayout;

        // This array holds the light positions
        // and will be updated via a push constant
        NativeList<Vector4> pushConstants = new NativeList<Vector4>(6, 6);


        public PushConstantsExample()
        {
            zoom = -30.0f;
            zoomSpeed = 2.5f;
            rotationSpeed = 0.5f;
            timerSpeed *= 0.5f;
            rotation = new Vector3(-32.5f, 45.0f, 0.0f);
            // enableTextOverlay = true;
            title = "Vulkan Example - Push constants";
        }

        public void Dispose()
        {
            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class
            vkDestroyPipeline(device, pipelines_solid, null);

            vkDestroyPipelineLayout(device, pipelineLayout, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayout, null);

            models_scene.destroy();

            uniformBuffer.destroy();
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

                vkCmdBeginRenderPass(drawCmdBuffers[i], &renderPassBeginInfo, VkSubpassContents.Inline);

                VkViewport viewport = Initializers.viewport((float)width, (float)height, 0.0f, 1.0f);
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);

                VkRect2D scissor = Initializers.rect2D(width, height, 0, 0);
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, &scissor);

                // Update light positions
                // w component = light radius scale
                const float r = 7.5f;
                float sin_t = (float)Math.Sin(Util.DegreesToRadians(timer * 360));
                float cos_t = (float)Math.Cos(Util.DegreesToRadians(timer * 360));
                const float y = -4.0f;
                pushConstants[0] = new Vector4(r * 1.1f * sin_t, y, r * 1.1f * cos_t, 1.0f);
                pushConstants[1] = new Vector4(-r * sin_t, y, -r * cos_t, 1.0f);
                pushConstants[2] = new Vector4(r * 0.85f * sin_t, y, -sin_t * 2.5f, 1.5f);
                pushConstants[3] = new Vector4(0.0f, y, r * 1.25f * cos_t, 1.5f);
                pushConstants[4] = new Vector4(r * 2.25f * cos_t, y, 0.0f, 1.25f);
                pushConstants[5] = new Vector4(r * 2.5f * (float)Math.Cos(Util.DegreesToRadians(timer * 360)), y, r * 2.5f * sin_t, 1.25f);

                // Submit via push constant (rather than a UBO)
                vkCmdPushConstants(
                    drawCmdBuffers[i],
                    pipelineLayout,
                    VkShaderStageFlags.Vertex,
                    0,
                    pushConstants.Count * (uint)sizeof(Vector4),
                    pushConstants.Data.ToPointer());

                vkCmdBindPipeline(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelines_solid);
                vkCmdBindDescriptorSets(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, pipelineLayout, 0, 1, ref descriptorSet, 0, null);

                ulong offsets = 0;
                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_scene.vertices.buffer, &offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], models_scene.indices.buffer, 0, VkIndexType.Uint32);

                vkCmdDrawIndexed(drawCmdBuffers[i], models_scene.indexCount, 1, 0, 0, 0);

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void loadAssets()
        {
            models_scene.loadFromFile(getAssetPath() + "models/samplescene.dae", vertexLayout, 0.35f, vulkanDevice, queue);
        }

        void setupVertexDescriptions()
        {
            // Binding description
            vertices_bindingDescriptions.Count = 1;
            vertices_bindingDescriptions[0] =
                Initializers.vertexInputBindingDescription(
                    VERTEX_BUFFER_BIND_ID,
                    vertexLayout.stride(),
                    VkVertexInputRate.Vertex);

            // Attribute descriptions
            // Describes memory layout and shader positions
            vertices_attributeDescriptions.Count = 4;
            // Location 0 : Position
            vertices_attributeDescriptions[0] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    0,
                    VkFormat.R32g32b32Sfloat,
                    0);
            // Location 1 : Normal
            vertices_attributeDescriptions[1] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    1,
                    VkFormat.R32g32b32Sfloat,
                    sizeof(float) * 3);
            // Location 2 : Texture coordinates
            vertices_attributeDescriptions[2] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    2,
                    VkFormat.R32g32Sfloat,
                    sizeof(float) * 6);
            // Location 3 : Color
            vertices_attributeDescriptions[3] =
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    3,
                    VkFormat.R32g32b32Sfloat,
                    sizeof(float) * 8);

            vertices_inputState = Initializers.pipelineVertexInputStateCreateInfo();
            vertices_inputState.vertexBindingDescriptionCount = vertices_bindingDescriptions.Count;
            vertices_inputState.pVertexBindingDescriptions = (VkVertexInputBindingDescription*)vertices_bindingDescriptions.Data;
            vertices_inputState.vertexAttributeDescriptionCount = vertices_attributeDescriptions.Count;
            vertices_inputState.pVertexAttributeDescriptions = (VkVertexInputAttributeDescription*)vertices_attributeDescriptions.Data;
        }

        void setupDescriptorPool()
        {
            // Example uses one ubo 
            VkDescriptorPoolSize poolSizes = Initializers.descriptorPoolSize(VkDescriptorType.UniformBuffer, 1);

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                Initializers.descriptorPoolCreateInfo(
                    1,
                    &poolSizes,
                    2);

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, out descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            VkDescriptorSetLayoutBinding setLayoutBindings =
                // Binding 0 : Vertex shader uniform buffer
                Initializers.descriptorSetLayoutBinding(
                    VkDescriptorType.UniformBuffer,
                    VkShaderStageFlags.Vertex,
                    0);

            VkDescriptorSetLayoutCreateInfo descriptorLayout =
                Initializers.descriptorSetLayoutCreateInfo(
                    &setLayoutBindings,
                    1);

            Util.CheckResult(vkCreateDescriptorSetLayout(device, &descriptorLayout, null, out descriptorSetLayout));

            var dsl = descriptorSetLayout;
            VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo =
                Initializers.pipelineLayoutCreateInfo(
                    &dsl,
                    1);

            // Define push constant
            // Example uses six light positions as push constants
            // 6 * 4 * 4 = 96 bytes
            // Spec requires a minimum of 128 bytes, bigger values
            // need to be checked against maxPushConstantsSize
            // But even at only 128 bytes, lots of stuff can fit 
            // inside push constants
            VkPushConstantRange pushConstantRange =
                Initializers.pushConstantRange(
                    VkShaderStageFlags.Vertex,
                    pushConstants.Count * (uint)sizeof(Vector4),
                    0);

            // Push constant ranges are part of the pipeline layout
            pipelineLayoutCreateInfo.pushConstantRangeCount = 1;
            pipelineLayoutCreateInfo.pPushConstantRanges = &pushConstantRange;

            Util.CheckResult(vkCreatePipelineLayout(device, &pipelineLayoutCreateInfo, null, out pipelineLayout));
        }

        void setupDescriptorSet()
        {
            var dsl = descriptorSetLayout;
            VkDescriptorSetAllocateInfo allocInfo =
                Initializers.descriptorSetAllocateInfo(
                    descriptorPool,
                    &dsl,
                    1);

            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSet));

            var descriptor = uniformBuffer.descriptor;
            // Binding 0 : Vertex shader uniform buffer
            VkWriteDescriptorSet writeDescriptorSet =
                Initializers.writeDescriptorSet(
                    descriptorSet,
                    VkDescriptorType.UniformBuffer,
                    0,
                    &descriptor);

            vkUpdateDescriptorSets(device, 1, &writeDescriptorSet, 0, null);
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
                    0xf,
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

            // Solid rendering pipeline
            // Load shaders
            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStages = new FixedArray2<VkPipelineShaderStageCreateInfo>(
                loadShader(getAssetPath() + "shaders/pushconstants/lights.vert.spv", VkShaderStageFlags.Vertex),
                loadShader(getAssetPath() + "shaders/pushconstants/lights.frag.spv", VkShaderStageFlags.Fragment));

            VkGraphicsPipelineCreateInfo pipelineCreateInfo =
                Initializers.pipelineCreateInfo(
                    pipelineLayout,
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

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_solid));
        }

        void prepareUniformBuffers()
        {
            // Vertex shader uniform buffer block
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
            // Vertex shader
            _uboVS.projection = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(60.0f), (float)width / (float)height, 0.001f, 256.0f);
            Matrix4x4 viewMatrix = Matrix4x4.CreateTranslation(new Vector3(0, 2, zoom));

            _uboVS.model = viewMatrix;
            _uboVS.model = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotation.X)) * _uboVS.model;
            _uboVS.model = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotation.Y)) * _uboVS.model;
            _uboVS.model = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotation.Z)) * _uboVS.model;

            var localUbovs = _uboVS;
            Unsafe.CopyBlock(uniformBuffer.mapped, &localUbovs, (uint)sizeof(UboVS));
        }

        void draw()
        {
            base.prepareFrame();

            // Command buffer to be sumitted to the queue
            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = (VkCommandBuffer*)drawCmdBuffers.GetAddress(currentBuffer);

            // Submit to queue
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, VkFence.Null));

            base.submitFrame();
        }

        public override void Prepare()
        {
            base.Prepare();

            // Check requested push constant size against hardware limit
            // Specs require 128 bytes, so if the Device complies our push constant buffer should always fit into memory		
            Debug.Assert(pushConstants.Count * (uint)sizeof(Vector4) <= vulkanDevice.properties.limits.maxPushConstantsSize);

            loadAssets();
            setupVertexDescriptions();
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
            if (!paused)
            {
                reBuildCommandBuffers();
            }
        }

        protected override void viewChanged()
        {
            updateUniformBuffers();
        }

        public static void Main() => new PushConstantsExample().ExampleMain();
    }
}
