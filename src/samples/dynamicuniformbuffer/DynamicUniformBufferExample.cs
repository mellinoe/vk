// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: dynamicuniformbuffer/dynamicuniformbuffer.cpp

/*
* Vulkan Example - Dynamic uniform buffers
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*
* Summary:
* Demonstrates the use of dynamic uniform buffers.
*
* Instead of using one uniform buffer per-object, this example allocates one big uniform buffer
* with respect to the alignment reported by the device via minUniformBufferOffsetAlignment that
* contains all matrices for the objects in the scene.
*
* The used descriptor type VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC then allows to set a dynamic
* offset used to pass data from the single uniform buffer to the connected shader binding point.
*/

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vulkan;
using static Vulkan.VulkanNative;
using static Vulkan.RawConstants;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Vk.Samples
{
    public unsafe class DynamicUniformBufferExample : VulkanExampleBase
    {
        private const uint VERTEX_BUFFER_BIND_ID = 0;
        private const uint OBJECT_INSTANCES = 125;

        // Vertex layout for this example
        struct Vertex
        {
            public Vector3 pos;
            public Vector3 color;
        };

        // Wrapper functions for aligned memory allocation
        // There is currently no standard for this in C++ that works across all platforms and vendors, so we abstract this
        void* alignedAlloc(IntPtr size, IntPtr alignment)
        {
            return Marshal.AllocHGlobal(size).ToPointer();
            /*
            void* data = null;
#if defined(_MSC_VER) || defined(__MINGW32__)
	data = _aligned_malloc(size, alignment);
#else
            int res = posix_memalign(&data, alignment, size);
            if (res != 0)
                data = null;
#endif
            return data;
            */
        }

        void alignedFree(void* data)
        {
            Marshal.FreeHGlobal(new IntPtr(data));
        }

        VkPipelineVertexInputStateCreateInfo vertices_inputState;
        NativeList<VkVertexInputBindingDescription> vertices_bindingDescriptions = new NativeList<VkVertexInputBindingDescription>();
        NativeList<VkVertexInputAttributeDescription> vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>();

        vksBuffer vertexBuffer = new vksBuffer();
        vksBuffer indexBuffer = new vksBuffer();
        uint indexCount;

        vksBuffer uniformBuffers_view = new vksBuffer();
        vksBuffer uniformBuffers_dynamic = new vksBuffer();

        struct UboVS
        {

            public Matrix4x4 projection;
            public Matrix4x4 view;
        }
        UboVS uboVS;
        // Store random per-object rotations
        Vector3[] rotations = new Vector3[OBJECT_INSTANCES];
        Vector3[] rotationSpeeds = new Vector3[OBJECT_INSTANCES];

        // One big uniform buffer that contains all matrices
        // Note that we need to manually allocate the data to cope for GPU-specific uniform buffer offset alignments
        Matrix4x4* uboDataDynamic_model;

        VkPipeline pipeline;
        VkPipelineLayout pipelineLayout;
        VkDescriptorSet descriptorSet;
        VkDescriptorSetLayout descriptorSetLayout;

        float animationTimer = 0.0f;

        IntPtr dynamicAlignment;


        public DynamicUniformBufferExample()
        {
            title = "Vulkan Example - Dynamic uniform buffers";
            // enableTextOverlay = true;
            camera.type = Camera.CameraType.lookat;
            camera.setPosition(new Vector3(0.0f, 0.0f, -30.0f));
            camera.setRotation(new Vector3(0.0f));
            camera.setPerspective(60.0f, (float)width / (float)height, 0.1f, 256.0f);
        }

        public void Dispose()
        {
            if (uboDataDynamic_model != null)
            {
                alignedFree(uboDataDynamic_model);
            }

            // Clean up used Vulkan resources 
            // Note : Inherited destructor cleans up resources stored in base class
            vkDestroyPipeline(device, pipeline, null);

            vkDestroyPipelineLayout(device, pipelineLayout, null);
            vkDestroyDescriptorSetLayout(device, descriptorSetLayout, null);

            vertexBuffer.destroy();
            indexBuffer.destroy();

            uniformBuffers_view.destroy();
            uniformBuffers_dynamic.destroy();
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
                renderPassBeginInfo.framebuffer = frameBuffers[i];

                Util.CheckResult(vkBeginCommandBuffer(drawCmdBuffers[i], &cmdBufInfo));

                vkCmdBeginRenderPass(drawCmdBuffers[i], &renderPassBeginInfo, VK_SUBPASS_CONTENTS_INLINE);

                VkViewport viewport = Initializers.viewport((float)width, (float)height, 0.0f, 1.0f);
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);

                VkRect2D scissor = Initializers.rect2D(width, height, 0, 0);
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, &scissor);

                vkCmdBindPipeline(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipeline);

                ulong offsets = 0;
                vkCmdBindVertexBuffers(drawCmdBuffers[i], VERTEX_BUFFER_BIND_ID, 1, ref vertexBuffer.buffer, &offsets);
                vkCmdBindIndexBuffer(drawCmdBuffers[i], indexBuffer.buffer, 0, VK_INDEX_TYPE_UINT32);

                // Render multiple objects using different model matrices by dynamically offsetting into one uniform buffer
                for (uint j = 0; j < OBJECT_INSTANCES; j++)
                {
                    // One dynamic offset per dynamic descriptor to offset into the ubo containing all model matrices
                    uint dynamicOffset = j * (uint)(dynamicAlignment);
                    // Bind the descriptor set for rendering a mesh using the dynamic offset
                    vkCmdBindDescriptorSets(drawCmdBuffers[i], VK_PIPELINE_BIND_POINT_GRAPHICS, pipelineLayout, 0, 1, ref descriptorSet, 1, &dynamicOffset);

                    vkCmdDrawIndexed(drawCmdBuffers[i], indexCount, 1, 0, 0, 0);
                }

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void draw()
        {
            prepareFrame();

            // Command buffer to be sumitted to the queue
            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = (VkCommandBuffer*)drawCmdBuffers.GetAddress(currentBuffer);

            // Submit to queue
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, VkFence.Null));

            submitFrame();
        }

        void generateCube()
        {
            // Setup vertices indices for a colored cube
            NativeList<Vertex> vertices = new NativeList<Vertex>
            {
                { new Vertex { pos = new Vector3(-1.0f, -1.0f, 1.0f), color = new Vector3(1.0f, 0.0f, 0.0f) } },
                { new Vertex { pos = new Vector3(1.0f, -1.0f, 1.0f), color = new Vector3(0.0f, 1.0f, 0.0f) } },
                { new Vertex { pos = new Vector3(1.0f, 1.0f, 1.0f), color = new Vector3(0.0f, 0.0f, 1.0f) } },
                { new Vertex { pos = new Vector3(-1.0f, 1.0f, 1.0f), color = new Vector3(0.0f, 0.0f, 0.0f) } },
                { new Vertex { pos = new Vector3(-1.0f, -1.0f, -1.0f), color = new Vector3(1.0f, 0.0f, 0.0f) } },
                { new Vertex { pos = new Vector3(1.0f, -1.0f, -1.0f), color = new Vector3(0.0f, 1.0f, 0.0f ) } },
                { new Vertex { pos = new Vector3(1.0f, 1.0f, -1.0f), color = new Vector3(0.0f, 0.0f, 1.0f) } },
                { new Vertex { pos = new Vector3(-1.0f, 1.0f, -1.0f), color = new Vector3(0.0f, 0.0f, 0.0f) } }
            };

            NativeList<uint> indices = new NativeList<uint>
            {
                0,1,2, 2,3,0, 1,5,6, 6,2,1, 7,6,5, 5,4,7, 4,0,3, 3,7,4, 4,5,1, 1,0,4, 3,2,6, 6,7,3,
            };

            indexCount = indices.Count;

            // Create buffers
            // For the sake of simplicity we won't stage the vertex data to the gpu memory
            // Vertex buffer
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                vertexBuffer,
                (ulong)(vertices.Count * sizeof(Vertex)),
                vertices.Data));
            // Index buffer
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                indexBuffer,
                indices.Count * sizeof(uint),
                indices.Data));
        }

        void setupVertexDescriptions()
        {
            // Binding description
            vertices_bindingDescriptions = new NativeList<VkVertexInputBindingDescription>
            {
                Initializers.vertexInputBindingDescription(VERTEX_BUFFER_BIND_ID, (uint)sizeof(Vertex), VK_VERTEX_INPUT_RATE_VERTEX),
            };

            // Attribute descriptions
            vertices_attributeDescriptions = new NativeList<VkVertexInputAttributeDescription>
            {
                Initializers.vertexInputAttributeDescription(VERTEX_BUFFER_BIND_ID, 0, VK_FORMAT_R32G32B32_SFLOAT, 0),	// Location 0 : Position
			    Initializers.vertexInputAttributeDescription(VERTEX_BUFFER_BIND_ID, 1, VK_FORMAT_R32G32B32_SFLOAT, (uint)sizeof(Vector3)),	// Location 1 : Color
		    };

            vertices_inputState = Initializers.pipelineVertexInputStateCreateInfo();
            vertices_inputState.vertexBindingDescriptionCount = vertices_bindingDescriptions.Count;
            vertices_inputState.pVertexBindingDescriptions = (VkVertexInputBindingDescription*)vertices_bindingDescriptions.Data;
            vertices_inputState.vertexAttributeDescriptionCount = vertices_attributeDescriptions.Count;
            vertices_inputState.pVertexAttributeDescriptions = (VkVertexInputAttributeDescription*)vertices_attributeDescriptions.Data;
        }

        void setupDescriptorPool()
        {
            // Example uses one ubo and one image sampler
            FixedArray3<VkDescriptorPoolSize> poolSizes = new FixedArray3<VkDescriptorPoolSize>(
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 1),
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC, 1),
                Initializers.descriptorPoolSize(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, 1));

            VkDescriptorPoolCreateInfo descriptorPoolInfo =
                Initializers.descriptorPoolCreateInfo(
                    (uint)(poolSizes.Count),
                    &poolSizes.First,
                    2);

            Util.CheckResult(vkCreateDescriptorPool(device, &descriptorPoolInfo, null, out descriptorPool));
        }

        void setupDescriptorSetLayout()
        {
            FixedArray3<VkDescriptorSetLayoutBinding> setLayoutBindings = new FixedArray3<VkDescriptorSetLayoutBinding>(
                Initializers.descriptorSetLayoutBinding(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, VK_SHADER_STAGE_VERTEX_BIT, 0),
                Initializers.descriptorSetLayoutBinding(VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC, VK_SHADER_STAGE_VERTEX_BIT, 1),
                Initializers.descriptorSetLayoutBinding(VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, VK_SHADER_STAGE_FRAGMENT_BIT, 2));

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

            var descriptor0 = uniformBuffers_view.descriptor;
            var descriptor1 = uniformBuffers_dynamic.descriptor;
            FixedArray2<VkWriteDescriptorSet> writeDescriptorSets = new FixedArray2<VkWriteDescriptorSet>(
                // Binding 0 : Projection/View matrix uniform buffer			
                Initializers.writeDescriptorSet(descriptorSet, VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, 0, &descriptor0),
                // Binding 1 : Instance matrix as dynamic uniform buffer
                Initializers.writeDescriptorSet(descriptorSet, VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC, 1, &descriptor1));

            vkUpdateDescriptorSets(device, writeDescriptorSets.Count, &writeDescriptorSets.First, 0, null);
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

            FixedArray2<VkDynamicState> dynamicStateEnables = new FixedArray2<VkDynamicState>(
                VK_DYNAMIC_STATE_VIEWPORT,
                VK_DYNAMIC_STATE_SCISSOR);

            VkPipelineDynamicStateCreateInfo dynamicState =
                Initializers.pipelineDynamicStateCreateInfo(
                    &dynamicStateEnables.First,
                    dynamicStateEnables.Count,
                    0);

            // Load shaders
            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStages = new FixedArray2<VkPipelineShaderStageCreateInfo>(
                loadShader(getAssetPath() + "shaders/dynamicuniformbuffer/base.vert.spv", VK_SHADER_STAGE_VERTEX_BIT),
                loadShader(getAssetPath() + "shaders/dynamicuniformbuffer/base.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT));

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

            Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, &pipelineCreateInfo, null, out pipeline));
        }

        // Prepare and initialize uniform buffer containing shader uniforms
        void prepareUniformBuffers()
        {
            // Allocate data for the dynamic uniform buffer object
            // We allocate this manually as the alignment of the offset differs between GPUs

            // Calculate required alignment depending on device limits
            ulong uboAlignment = vulkanDevice.properties.limits.minUniformBufferOffsetAlignment;
            dynamicAlignment = (IntPtr)(((ulong)sizeof(Matrix4x4) / uboAlignment) * uboAlignment + (((ulong)sizeof(Matrix4x4) % uboAlignment) > 0 ? uboAlignment : 0));

            IntPtr bufferSize = (IntPtr)(OBJECT_INSTANCES * (ulong)dynamicAlignment);

            uboDataDynamic_model = (Matrix4x4*)alignedAlloc(bufferSize, dynamicAlignment);
            Debug.Assert(uboDataDynamic_model != null);

            Console.WriteLine("minUniformBufferOffsetAlignment = " + uboAlignment);
            Console.WriteLine("dynamicAlignment = " + dynamicAlignment);

            // Vertex shader uniform buffer block

            // Static shared uniform buffer object with projection and view matrix
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
                uniformBuffers_view,
                (ulong)sizeof(UboVS)));

            // Uniform buffer object with per-object matrices
            Util.CheckResult(vulkanDevice.createBuffer(
                VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT,
                VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT,
                uniformBuffers_dynamic,
                (ulong)bufferSize));

            // Map persistent
            Util.CheckResult(uniformBuffers_view.map());
            Util.CheckResult(uniformBuffers_dynamic.map());

            // Prepare per-object matrices with offsets and random rotations
            Random rndGen = new Random();
            Func<Random, float> rndDist = rand => (float)(rand.NextDouble() * 2 - 1.0);
            for (uint i = 0; i < OBJECT_INSTANCES; i++)
            {
                rotations[i] = new Vector3(rndDist(rndGen), rndDist(rndGen), rndDist(rndGen)) * 2.0f * (float)Math.PI;
                rotationSpeeds[i] = new Vector3(rndDist(rndGen), rndDist(rndGen), rndDist(rndGen));
            }

            updateUniformBuffers();
            updateDynamicUniformBuffer(true);
        }

        void updateUniformBuffers()
        {
            // Fixed ubo with projection and view matrices
            uboVS.projection = camera.matrices_perspective;
            uboVS.view = camera.matrices_view;

            var local = uboVS;
            Unsafe.CopyBlock(uniformBuffers_view.mapped, &local, (uint)sizeof(UboVS));
        }

        void updateDynamicUniformBuffer(bool force = false)
        {
            // Update at max. 60 fps
            animationTimer += (frameTimer * 100.0f);
            if ((animationTimer <= 1.0f / 60.0f) && (!force))
            {
                return;
            }

            // Dynamic ubo with per-object model matrices indexed by offsets in the command buffer
            uint dim = (uint)(Math.Pow(OBJECT_INSTANCES, (1.0f / 3.0f)));
            Vector3 offset = new Vector3(5.0f);

            for (uint x = 0; x < dim; x++)
            {
                for (uint y = 0; y < dim; y++)
                {
                    for (uint z = 0; z < dim; z++)
                    {
                        uint index = x * dim * dim + y * dim + z;

                        // Aligned offset
                        Matrix4x4* modelMat = (Matrix4x4*)(((ulong)uboDataDynamic_model + (index * (ulong)dynamicAlignment)));

                        // Update rotations
                        rotations[index] += animationTimer * rotationSpeeds[index];

                        // Update matrices
                        Vector3 pos = new Vector3(-((dim * offset.X) / 2.0f) + offset.X / 2.0f + x * offset.X, -((dim * offset.Y) / 2.0f) + offset.Y / 2.0f + y * offset.Y, -((dim * offset.Z) / 2.0f) + offset.Z / 2.0f + z * offset.Z);
                        *modelMat = Matrix4x4.CreateTranslation(pos);
                        *modelMat = Matrix4x4.CreateRotationX(Util.DegreesToRadians(rotations[index].X)) * *modelMat;
                        *modelMat = Matrix4x4.CreateRotationY(Util.DegreesToRadians(rotations[index].Y)) * *modelMat;
                        *modelMat = Matrix4x4.CreateRotationZ(Util.DegreesToRadians(rotations[index].Z)) * *modelMat;
                    }
                }
            }

            animationTimer = 0.0f;

            Unsafe.CopyBlock(uniformBuffers_dynamic.mapped, uboDataDynamic_model, (uint)uniformBuffers_dynamic.size);
            // Flush to make changes visible to the host 
            VkMappedMemoryRange memoryRange = Initializers.mappedMemoryRange();
            memoryRange.memory = uniformBuffers_dynamic.memory;
            memoryRange.size = uniformBuffers_dynamic.size;
            vkFlushMappedMemoryRanges(device, 1, &memoryRange);
        }

        public override void Prepare()
        {
            base.Prepare();
            generateCube();
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
                updateDynamicUniformBuffer();
        }

        protected override void viewChanged()
        {
            updateUniformBuffers();
        }

        public static void Main() => new DynamicUniformBufferExample().ExampleMain();
    }
}
