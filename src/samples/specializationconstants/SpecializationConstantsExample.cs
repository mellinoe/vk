// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: specializationconstants/specializationconstants.cpp

/*
* Vulkan Example - Shader specialization constants
*
* For details see https://www.khronos.org/registry/vulkan/specs/misc/GL_KHR_vulkan_glsl.txt
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

namespace Vk.Samples
{
    public unsafe class SpecializationConstantsExample : VulkanExampleBase
    {
        private const uint VERTEX_BUFFER_BIND_ID = 0;

        private VkPipelineVertexInputStateCreateInfo vertices_inputState;
        private NativeList<VkVertexInputBindingDescription> vertices_bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
        private NativeList<VkVertexInputAttributeDescription> vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();

        // Vertex layout for the models
        vksVertexLayout vertexLayout = new vksVertexLayout(
            VertexComponent.VERTEX_COMPONENT_POSITION,
            VertexComponent.VERTEX_COMPONENT_NORMAL,
            VertexComponent.VERTEX_COMPONENT_UV,
            VertexComponent.VERTEX_COMPONENT_COLOR);

        vksModel models_cube = new vksModel();
        vksTexture2D textures_colormap = new vksTexture2D();
        vksBuffer uniformBuffer = new vksBuffer();

        // Same uniform buffer layout as shader
        struct UboVS
        {
            public Matrix4x4 projection;
            public Matrix4x4 modelView;
            public Vector4 lightPos;
        }
        private UboVS uboVS = new UboVS() { lightPos = new Vector4(0, -2, 1, 0) };

        VkPipelineLayout pipelineLayout;
        VkDescriptorSet descriptorSet;
        VkDescriptorSetLayout descriptorSetLayout;

        VkPipeline pipelines_phong;
        VkPipeline pipelines_toon;
        VkPipeline pipelines_textured;

        public SpecializationConstantsExample()
        {
            title = "Vulkan Example - Specialization constants";
            // enableTextOverlay = true;
            camera.type = Camera.CameraType.lookat;
            camera.setPerspective(60.0f, ((float)width / 3.0f) / (float)height, 0.1f, 512.0f);
            camera.setRotation(new Vector3(-40.0f, -90.0f, 0.0f));
            camera.setTranslation(new Vector3(0.0f, 0.0f, -2.0f));
        }

        public void Dispose()
        {
            vkDestroyPipeline(device, pipelines_phong, null);
            vkDestroyPipeline(device, pipelines_textured, null);
            vkDestroyPipeline(device, pipelines_toon, null);

            vkDestroyPipelineLayout(device, pipelineLayout, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayout, null);

            models_cube.destroy();
            textures_colormap.destroy();
            uniformBuffer.destroy();
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

                vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayout, 0, 1, ref descriptorSet, 0, null);

                ulong offsets = 0;
                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref models_cube.vertices.buffer, &offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], models_cube.indices.buffer, 0, VK_INDEX_TYPE_UINT32);

                // Left
                viewport.width = (float)width / 3.0f;
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_phong);

                vkCmdDrawIndexed(drawCmdBuffers[i], models_cube.indexCount, 1, 0, 0, 0);

                // Center
                viewport.x = (float)width / 3.0f;
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_toon);
                vkCmdDrawIndexed(drawCmdBuffers[i], models_cube.indexCount, 1, 0, 0, 0);

                // Right
                viewport.x = (float)width / 3.0f + (float)width / 3.0f;
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);
                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelines_textured);
                vkCmdDrawIndexed(drawCmdBuffers[i], models_cube.indexCount, 1, 0, 0, 0);

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void loadAssets()
        {
            models_cube.loadFromFile(getAssetPath() + "models/color_teapot_spheres.dae", vertexLayout, 0.1f, vulkanDevice, queue);
            textures_colormap.loadFromFile(getAssetPath() + "textures/metalplate_nomips_rgba.ktx", VK_FORMAT_R8G8B8A8_UNORM, vulkanDevice, queue);
        }

        void setupVertexDescriptions()
        {
            // Binding description
            vertices_bindingDescriptions.Count = 1;
            vertices_bindingDescriptions[0] = Initializers.vertexInputBindingDescription(VERTEX_BUFFER_BIND_ID, vertexLayout.stride(), VK_VERTEX_INPUT_RATE_VERTEX);

            // Attribute descriptions
            vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>
            {
                // Location 0 : Position
                Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    0,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    0),
			    // Location 1 : Color
			    Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    1,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    sizeof(float) * 3),
			    // Location 3 : Texture coordinates
			    Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    2,
                    VK_FORMAT_R32G32_SFLOAT,
                    sizeof(float) * 6),
			    // Location 2 : Normal
			    Initializers.vertexInputAttributeDescription(
                    VERTEX_BUFFER_BIND_ID,
                    3,
                    VK_FORMAT_R32G32B32_SFLOAT,
                    sizeof(float) * 8),
            };

            vertices_inputState = Initializers.pipelineVertexInputStateCreateInfo();
            vertices_inputState.vertexBindingDescriptionCount = vertices_bindingDescriptions.Count;
            vertices_inputState.pVertexBindingDescriptions = (VkVertexInputBindingDescription*)vertices_bindingDescriptions.Data;
            vertices_inputState.vertexAttributeDescriptionCount = vertices_attributeDescriptions.Count;
            vertices_inputState.pVertexAttributeDescriptions = (VkVertexInputAttributeDescription*)vertices_attributeDescriptions.Data;
        }

        void setupDescriptorPool()
        {
            FixedArray2<VkDescriptorPoolSize> poolSizes = new FixedArray2<VkDescriptorPoolSize>(
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 1),
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, 1));

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                Initializers.descriptorPoolCreateInfo(
                    (uint)(poolSizes.Count),
                    &poolSizes.First,
                    1);

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, out descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            FixedArray2<VkDescriptorSetLayoutBinding> setLayoutBindings = new FixedArray2<VkDescriptorSetLayoutBinding>(
                Initializers.descriptorSetLayoutBinding(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, VK_SHADER_STAGE_VERTEX_BIT, 0),
                Initializers.descriptorSetLayoutBinding(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, VK_SHADER_STAGE_FRAGMENT_BIT, 1));

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

        void setupDescriptorSet()
        {
            var dsl = descriptorSetLayout;
            VkDescriptorSetAllocateInfo allocInfo =
                Initializers.descriptorSetAllocateInfo(
                    descriptorPool,
                    &dsl,
                    1);

            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out descriptorSet));

            var descriptor1 = uniformBuffer.descriptor;
            var descriptor2 = textures_colormap.descriptor;
            FixedArray2<VkWriteDescriptorSet> writeDescriptorSets = new FixedArray2<VkWriteDescriptorSet>(
                Initializers.writeDescriptorSet(descriptorSet, VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 0, &descriptor1),
                Initializers.writeDescriptorSet(descriptorSet, VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, 1, &descriptor2));

            vkUpdateDescriptorSets(device, (uint)(writeDescriptorSets.Count), &writeDescriptorSets.First, 0, null);
        }

        void preparePipelines()
        {
            VkPipelineInputAssemblyStateCreateInfo inputAssemblyState =
                Initializers.pipelineInputAssemblyStateCreateInfo(
                    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                    0,
                    False);

            VkPipelineRasterizationStateCreateInfo rasterizationState =
                Initializers.pipelineRasterizationStateCreateInfo(
                    VK_POLYGON_MODE_FILL,
                    VK_CULL_MODE_NONE,
                    VK_FRONT_FACE_CLOCKWISE,
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
                    VK_COMPARE_OP_LESS_OR_EQUAL);

            VkPipelineViewportStateCreateInfo viewportState =
                Initializers.pipelineViewportStateCreateInfo(1, 1, 0);

            VkPipelineMultisampleStateCreateInfo multisampleState =
                Initializers.pipelineMultisampleStateCreateInfo(
                    VK_SAMPLE_COUNT_1_BIT,
                    0);

            FixedArray3<VkDynamicState> dynamicStateEnables = new FixedArray3<VkDynamicState>(
                VK_DYNAMIC_STATE_VIEWPORT,
                VK_DYNAMIC_STATE_SCISSOR,
                VK_DYNAMIC_STATE_LINE_WIDTH);

            VkPipelineDynamicStateCreateInfo dynamicState =
                Initializers.pipelineDynamicStateCreateInfo(
                    &dynamicStateEnables.First,
                    dynamicStateEnables.Count,
                    0);

            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStages = new FixedArray2<VkPipelineShaderStageCreateInfo>();

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

            // Prepare specialization data

            SpecializationData specializationData = SpecializationData.Default;

            // Each shader constant of a shader stage corresponds to one map entry
            FixedArray2<VkSpecializationMapEntry> specializationMapEntries = new FixedArray2<VkSpecializationMapEntry>();
            // Shader bindings based on specialization constants are marked by the new "constant_id" layout qualifier:
            //	layout (constant_id = 0) const int LIGHTING_MODEL = 0;
            //	layout (constant_id = 1) const float PARAM_TOON_DESATURATION = 0.0f;

            // Map entry for the lighting model to be used by the fragment shader
            specializationMapEntries.First.constantID = 0;
            specializationMapEntries.First.size = (UIntPtr)sizeof(uint);
            specializationMapEntries.First.offset = 0;

            // Map entry for the toon shader parameter
            specializationMapEntries.Second.constantID = 1;
            specializationMapEntries.Second.size = (UIntPtr)sizeof(float);
            specializationMapEntries.Second.offset = (sizeof(uint));

            // Prepare specialization info block for the shader stage
            VkSpecializationInfo specializationInfo = new VkSpecializationInfo();
            specializationInfo.dataSize = (UIntPtr)sizeof(SpecializationData);
            specializationInfo.mapEntryCount = specializationMapEntries.Count;
            specializationInfo.pMapEntries = &specializationMapEntries.First;
            specializationInfo.pData = &specializationData;

            // Create pipelines
            // All pipelines will use the same "uber" shader and specialization constants to change branching and parameters of that shader
            shaderStages.First = loadShader(getAssetPath() + "shaders/specializationconstants/uber.vert.spv", VK_SHADER_STAGE_VERTEX_BIT);
            shaderStages.Second = loadShader(getAssetPath() + "shaders/specializationconstants/uber.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT);
            // Specialization info is assigned is part of the shader stage (modul) and must be set after creating the module and before creating the pipeline
            shaderStages.Second.pSpecializationInfo = &specializationInfo;

            // Solid phong shading
            specializationData.lightingModel = 0;

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_phong));

            // Phong and textured
            specializationData.lightingModel = 1;

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_toon));

            // Textured discard
            specializationData.lightingModel = 2;

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipelines_textured));
        }

        struct SpecializationData
        {
            // Sets the lighting model used in the fragment "uber" shader
            public uint lightingModel;
            // Parameter for the toon shading part of the fragment shader
            public float toonDesaturationFactor;
            public static SpecializationData Default => new SpecializationData() { toonDesaturationFactor = 0.5f };
        }

        // Prepare and initialize uniform buffer containing shader uniforms
        void prepareUniformBuffers()
        {
            // Create the vertex shader uniform buffer block
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffer,
                (ulong)sizeof(UboVS)));

            // Map persistent
            Util.CheckResult(uniformBuffer.map());

            updateUniformBuffers();
        }

        void updateUniformBuffers()
        {
            uboVS.projection = camera.matrices_perspective;
            uboVS.modelView = camera.matrices_view;

            var local = uboVS;
            Unsafe.CopyBlock(uniformBuffer.mapped, &local, (uint)sizeof(UboVS));
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
        }

        protected override void viewChanged()
        {
            updateUniformBuffers();
        }

        public static void Main() => new SpecializationConstantsExample().ExampleMain();
    }
}
