// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: triangle/triangle.cpp, 

/*
* Vulkan Example - Basic indexed triangle rendering
*
* Note:
*	This is a "pedal to the metal" example to show off how to get Vulkan up an displaying something
*	Contrary to the other examples, this one won't make use of helper functions or initializers
*	Except in a few cases (swap chain setup e.g.)
*
* Copyright (C) 2016-2017 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class TriangleExample : VulkanExampleBase
    {
        public VkSemaphore PresentCompleteSemaphore => _presentCompleteSemaphore;
        public VkSemaphore RenderCompleteSemaphore => _renderCompleteSemaphore;
        public NativeList<VkFence> waitFences { get; } = new NativeList<VkFence>();
        public VkDescriptorSetLayout DescriptorSetLayout => _descriptorSetLayout;
        public VkPipelineLayout PipelineLayout => _pipelineLayout;
        public VkDescriptorPool DescriptorPool { get; private set; }
        public VkDescriptorSet DescriptorSet => _descriptorSet;
        public VkPipeline Pipeline => _pipeline;

        public Vertices _Vertices;
        public Indices _Indices;
        public UniformBufferVS _UniformBufferVS;
        public UboVS _UboVS;

        // Default fence timeout in nanoseconds
        public const ulong DEFAULT_FENCE_TIMEOUT = 100000000000;
        private VkSemaphore _presentCompleteSemaphore;
        private VkSemaphore _renderCompleteSemaphore;
        private VkDescriptorSetLayout _descriptorSetLayout;
        private VkPipelineLayout _pipelineLayout;
        private VkDescriptorSet _descriptorSet;
        private VkPipeline _pipeline;

        public static void Main()
        {
            TriangleExample example = new TriangleExample();
            example.zoom = -2.5f;
            example.InitVulkan();
            example.SetupWin32Window();
            example.InitSwapchain();
            example.Prepare();
            example.RenderLoop();
        }

        public override void Prepare()
        {
            base.Prepare();
            PrepareSynchronizationPrimitives();
            PrepareVertices(true);
            PrepareUniformBuffers();
            SetupDescriptorSetLayout();
            preparePipelines();
            SetupDescriptorPool();
            SetupDescriptorSet();
            buildCommandBuffers();
            prepared = true;
        }

        // Create the Vulkan synchronization primitives used in this example
        private void PrepareSynchronizationPrimitives()
        {
            // Semaphores (Used for correct command ordering)
            VkSemaphoreCreateInfo semaphoreCreateInfo = VkSemaphoreCreateInfo.New();

            // Semaphore used to ensures that image presentation is complete before starting to submit again
            Util.CheckResult(vkCreateSemaphore(device, &semaphoreCreateInfo, null, out _presentCompleteSemaphore));

            // Semaphore used to ensures that all commands submitted have been finished before submitting the image to the queue
            Util.CheckResult(vkCreateSemaphore(device, &semaphoreCreateInfo, null, out _renderCompleteSemaphore));

            // Fences (Used to check draw command buffer completion)
            VkFenceCreateInfo fenceCreateInfo = VkFenceCreateInfo.New();
            // Create in signaled state so we don't wait on first render of each command buffer
            fenceCreateInfo.flags = VkFenceCreateFlags.Signaled;
            waitFences.Resize(drawCmdBuffers.Count);
            waitFences.Count = drawCmdBuffers.Count;
            for (uint i = 0; i < waitFences.Count; i++)
            {
                Util.CheckResult(vkCreateFence(device, ref fenceCreateInfo, null, (VkFence*)waitFences.GetAddress(i)));
            }
        }

        // Prepare vertex and index buffers for an indexed triangle
        // Also uploads them to Device local memory using staging and initializes vertex input and attribute binding to match the vertex shader
        public void PrepareVertices(bool useStagingBuffers)
        {
            // A note on memory management in Vulkan in general:
            //	This is a very complex topic and while it's fine for an example application to to small individual memory allocations that is not
            //	what should be done a real-world application, where you should allocate large chunkgs of memory at once isntead.

            // Setup vertices

            FixedArray3<Vertex> vertexData = new FixedArray3<Vertex>(
                new Vertex { Position = new Vector3(1f, 1f, 0f), Color = new Vector3(1f, 0f, 0f) },
                new Vertex { Position = new Vector3(-1f, 1f, 0f), Color = new Vector3(0f, 1f, 0f) },
                new Vertex { Position = new Vector3(0f, -1f, 0f), Color = new Vector3(0f, 0f, 1f) });
            uint vertexBufferSize = vertexData.Count * (uint)sizeof(Vertex);

            // Setup indices
            TriangleIndices indexData = new TriangleIndices() { Index0 = 0, Index1 = 1, Index2 = 2 };
            _Indices.count = 3;
            uint indexBufferSize = _Indices.count * sizeof(uint);

            VkMemoryAllocateInfo memAlloc = VkMemoryAllocateInfo.New();
            VkMemoryRequirements memReqs;
            void* data;
            useStagingBuffers = true;
            if (useStagingBuffers)
            {
                // Static data like vertex and index buffer should be stored on the Device memory 
                // for optimal (and fastest) access by the GPU
                //
                // To achieve this we use so-called "staging buffers" :
                // - Create a buffer that's visible to the host (and can be mapped)
                // - Copy the data to this buffer
                // - Create another buffer that's local on the Device (VRAM) with the same size
                // - Copy the data from the host to the Device using a command buffer
                // - Delete the host visible (staging) buffer
                // - Use the Device local buffers for rendering

                StagingBuffers stagingBuffers;

                // Vertex buffer
                VkBufferCreateInfo vertexBufferInfo = VkBufferCreateInfo.New();
                vertexBufferInfo.size = vertexBufferSize;
                // Buffer is used as the copy source
                vertexBufferInfo.usage = VkBufferUsageFlags.TransferSrc;
                // Create a host-visible buffer to copy the vertex data to (staging buffer)
                Util.CheckResult(vkCreateBuffer(device, ref vertexBufferInfo, null, out stagingBuffers.vertices.buffer));

                vkGetBufferMemoryRequirements(device, stagingBuffers.vertices.buffer, out memReqs);
                memAlloc.allocationSize = memReqs.size;
                // Request a host visible memory type that can be used to copy our data do
                // Also request it to be coherent, so that writes are visible to the GPU right after unmapping the buffer
                memAlloc.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

                Util.CheckResult(vkAllocateMemory(device, ref memAlloc, null, out stagingBuffers.vertices.memory));
                // Map and copy
                Util.CheckResult(vkMapMemory(device, stagingBuffers.vertices.memory, 0, memAlloc.allocationSize, 0, &data));

                Unsafe.CopyBlock(data, Unsafe.AsPointer(ref vertexData), vertexBufferSize);

                vkUnmapMemory(device, stagingBuffers.vertices.memory);

                Util.CheckResult(vkBindBufferMemory(device, stagingBuffers.vertices.buffer, stagingBuffers.vertices.memory, 0));

                // Create a Device local buffer to which the (host local) vertex data will be copied and which will be used for rendering
                vertexBufferInfo.usage = (VkBufferUsageFlags.VertexBuffer | VkBufferUsageFlags.TransferDst);

                Util.CheckResult(vkCreateBuffer(device, ref vertexBufferInfo, null, out _Vertices.buffer));

                vkGetBufferMemoryRequirements(device, _Vertices.buffer, out memReqs);
                memAlloc.allocationSize = memReqs.size;
                memAlloc.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

                Util.CheckResult(vkAllocateMemory(device, &memAlloc, null, out _Vertices.memory));

                Util.CheckResult(vkBindBufferMemory(device, _Vertices.buffer, _Vertices.memory, 0));

                // Index buffer
                VkBufferCreateInfo indexbufferInfo = VkBufferCreateInfo.New();
                indexbufferInfo.size = indexBufferSize;
                indexbufferInfo.usage = VkBufferUsageFlags.TransferSrc;
                // Copy index data to a buffer visible to the host (staging buffer)
                Util.CheckResult(vkCreateBuffer(device, ref indexbufferInfo, null, out stagingBuffers.indices.buffer));

                vkGetBufferMemoryRequirements(device, stagingBuffers.indices.buffer, out memReqs);
                memAlloc.allocationSize = memReqs.size;
                memAlloc.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

                Util.CheckResult(vkAllocateMemory(device, ref memAlloc, null, out stagingBuffers.indices.memory));

                Util.CheckResult(vkMapMemory(device, stagingBuffers.indices.memory, 0, indexBufferSize, 0, &data));

                Unsafe.CopyBlock(data, &indexData, indexBufferSize);

                vkUnmapMemory(device, stagingBuffers.indices.memory);

                Util.CheckResult(vkBindBufferMemory(device, stagingBuffers.indices.buffer, stagingBuffers.indices.memory, 0));

                // Create destination buffer with Device only visibility
                indexbufferInfo.usage = (VkBufferUsageFlags.IndexBuffer | VkBufferUsageFlags.TransferDst);

                Util.CheckResult(vkCreateBuffer(device, &indexbufferInfo, null, out _Indices.buffer));

                vkGetBufferMemoryRequirements(device, _Indices.buffer, out memReqs);
                memAlloc.allocationSize = memReqs.size;
                memAlloc.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

                Util.CheckResult(vkAllocateMemory(device, &memAlloc, null, out _Indices.memory));

                Util.CheckResult(vkBindBufferMemory(device, _Indices.buffer, _Indices.memory, 0));

                VkCommandBufferBeginInfo cmdBufferBeginInfo = VkCommandBufferBeginInfo.New();

                // Buffer copies have to be submitted to a queue, so we need a command buffer for them
                // Note: Some devices offer a dedicated transfer queue (with only the transfer bit set) that may be faster when doing lots of copies
                VkCommandBuffer copyCmd = getCommandBuffer(true);

                // Put buffer region copies into command buffer
                VkBufferCopy copyRegion = new VkBufferCopy();

                // Vertex buffer
                copyRegion.size = vertexBufferSize;

                vkCmdCopyBuffer(copyCmd, stagingBuffers.vertices.buffer, _Vertices.buffer, 1, ref copyRegion);
                // Index buffer
                copyRegion.size = indexBufferSize;

                vkCmdCopyBuffer(copyCmd, stagingBuffers.indices.buffer, _Indices.buffer, 1, ref copyRegion);

                // Flushing the command buffer will also submit it to the queue and uses a fence to ensure that all commands have been executed before returning
                FlushCommandBuffer(copyCmd);

                // Destroy staging buffers
                // Note: Staging buffer must not be deleted before the copies have been submitted and executed
                vkDestroyBuffer(device, stagingBuffers.vertices.buffer, null);

                vkFreeMemory(device, stagingBuffers.vertices.memory, null);

                vkDestroyBuffer(device, stagingBuffers.indices.buffer, null);

                vkFreeMemory(device, stagingBuffers.indices.memory, null);
            }
            else
            {
                // Don't use staging
                // Create host-visible buffers only and use these for rendering. This is not advised and will usually result in lower rendering performance

                // Vertex buffer
                VkBufferCreateInfo vertexBufferInfo = VkBufferCreateInfo.New();
                vertexBufferInfo.size = vertexBufferSize;
                vertexBufferInfo.usage = VkBufferUsageFlags.VertexBuffer;

                // Copy vertex data to a buffer visible to the host
                Util.CheckResult(vkCreateBuffer(device, ref vertexBufferInfo, null, out _Vertices.buffer));

                vkGetBufferMemoryRequirements(device, _Vertices.buffer, out memReqs);
                memAlloc.allocationSize = memReqs.size;
                memAlloc.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible);

                Util.CheckResult(vkAllocateMemory(device, &memAlloc, null, out _Vertices.memory));

                Util.CheckResult(vkMapMemory(device, _Vertices.memory, 0, memAlloc.allocationSize, 0, &data));

                Unsafe.CopyBlock(data, Unsafe.AsPointer(ref vertexData), vertexBufferSize);

                vkUnmapMemory(device, _Vertices.memory);

                Util.CheckResult(vkBindBufferMemory(device, _Vertices.buffer, _Vertices.memory, 0));

                // Index buffer
                VkBufferCreateInfo indexbufferInfo = VkBufferCreateInfo.New();
                indexbufferInfo.size = indexBufferSize;
                indexbufferInfo.usage = VkBufferUsageFlags.IndexBuffer;

                // Copy index data to a buffer visible to the host
                Util.CheckResult(vkCreateBuffer(device, &indexbufferInfo, null, out _Indices.buffer));

                vkGetBufferMemoryRequirements(device, _Indices.buffer, &memReqs);
                memAlloc.allocationSize = memReqs.size;
                memAlloc.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible);

                Util.CheckResult(vkAllocateMemory(device, &memAlloc, null, out _Indices.memory));

                Util.CheckResult(vkMapMemory(device, _Indices.memory, 0, indexBufferSize, 0, &data));

                Unsafe.CopyBlock(data, &indexData, indexBufferSize);

                vkUnmapMemory(device, _Indices.memory);

                Util.CheckResult(vkBindBufferMemory(device, _Indices.buffer, _Indices.memory, 0));
            }

        }

        // This function is used to request a Device memory type that supports all the property flags we request (e.g. Device local, host visibile)
        // Upon success it will return the index of the memory type that fits our requestes memory properties
        // This is necessary as implementations can offer an arbitrary number of memory types with different
        // memory properties. 
        // You can check http://vulkan.gpuinfo.org/ for details on different memory configurations
        uint GetMemoryTypeIndex(uint typeBits, VkMemoryPropertyFlags properties)
        {
            // Iterate over all memory types available for the Device used in this example
            for (uint i = 0; i < DeviceMemoryProperties.memoryTypeCount; i++)
            {
                if ((typeBits & 1) == 1)
                {
                    if ((DeviceMemoryProperties.GetMemoryType(i).propertyFlags & properties) == properties)
                    {
                        return i;
                    }
                }
                typeBits >>= 1;
            }

            throw new InvalidOperationException("Could not find a suitable memory type!");
        }

        // Get a new command buffer from the command pool
        // If begin is true, the command buffer is also started so we can start adding commands
        private VkCommandBuffer getCommandBuffer(bool begin)
        {
            VkCommandBufferAllocateInfo cmdBufAllocateInfo = VkCommandBufferAllocateInfo.New();
            cmdBufAllocateInfo.commandPool = cmdPool;
            cmdBufAllocateInfo.level = VkCommandBufferLevel.Primary;
            cmdBufAllocateInfo.commandBufferCount = 1;

            Util.CheckResult(vkAllocateCommandBuffers(device, ref cmdBufAllocateInfo, out VkCommandBuffer cmdBuffer));

            // If requested, also start the new command buffer
            if (begin)
            {
                VkCommandBufferBeginInfo cmdBufInfo = VkCommandBufferBeginInfo.New();
                Util.CheckResult(vkBeginCommandBuffer(cmdBuffer, ref cmdBufInfo));
            }

            return cmdBuffer;
        }

        // End the command buffer and submit it to the queue
        // Uses a fence to ensure command buffer has finished executing before deleting it
        void FlushCommandBuffer(VkCommandBuffer commandBuffer)
        {
            Debug.Assert(commandBuffer != NullHandle);

            Util.CheckResult(vkEndCommandBuffer(commandBuffer));

            VkSubmitInfo submitInfo = VkSubmitInfo.New();
            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = &commandBuffer;

            // Create fence to ensure that the command buffer has finished executing
            VkFenceCreateInfo fenceCreateInfo = VkFenceCreateInfo.New();
            Util.CheckResult(vkCreateFence(device, &fenceCreateInfo, null, out VkFence fence));

            // Submit to the queue
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, fence));
            // Wait for the fence to signal that command buffer has finished executing
            Util.CheckResult(vkWaitForFences(device, 1, ref fence, True, DEFAULT_FENCE_TIMEOUT));

            vkDestroyFence(device, fence, null);
            vkFreeCommandBuffers(device, cmdPool, 1, &commandBuffer);
        }

        public void PrepareUniformBuffers()
        {
            // Prepare and initialize a uniform buffer block containing shader uniforms
            // Single uniforms like in OpenGL are no longer present in Vulkan. All Shader uniforms are passed via uniform buffer blocks
            VkMemoryRequirements memReqs;

            // Vertex shader uniform buffer block
            VkMemoryAllocateInfo allocInfo = VkMemoryAllocateInfo.New();
            allocInfo.pNext = null;
            allocInfo.allocationSize = 0;
            allocInfo.memoryTypeIndex = 0;

            VkBufferCreateInfo bufferInfo = VkBufferCreateInfo.New();
            bufferInfo.size = (ulong)sizeof(UboVS);
            // This buffer will be used as a uniform buffer
            bufferInfo.usage = VkBufferUsageFlags.UniformBuffer;

            // Create a new buffer
            Util.CheckResult(vkCreateBuffer(device, &bufferInfo, null, out _UniformBufferVS.buffer));

            // Get memory requirements including size, alignment and memory type 
            vkGetBufferMemoryRequirements(device, _UniformBufferVS.buffer, out memReqs);
            allocInfo.allocationSize = memReqs.size;
            // Get the memory type index that supports host visibile memory access
            // Most implementations offer multiple memory types and selecting the correct one to allocate memory from is crucial
            // We also want the buffer to be host coherent so we don't have to flush (or sync after every update.
            // Note: This may affect performance so you might not want to do this in a real world application that updates buffers on a regular base
            allocInfo.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCached);
            // Allocate memory for the uniform buffer
            Util.CheckResult(vkAllocateMemory(device, &allocInfo, null, out _UniformBufferVS.memory));
            // Bind memory to buffer
            Util.CheckResult(vkBindBufferMemory(device, _UniformBufferVS.buffer, _UniformBufferVS.memory, 0));

            // Store information in the uniform's descriptor that is used by the descriptor set
            _UniformBufferVS.descriptor.buffer = _UniformBufferVS.buffer;
            _UniformBufferVS.descriptor.offset = 0;
            _UniformBufferVS.descriptor.range = (ulong)sizeof(UboVS);

            UpdateUniformBuffers();
        }

        private void UpdateUniformBuffers()
        {
            // Update matrices
            _UboVS.projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(Util.DegreesToRadians(60f), (float)width / (float)height, 0.1f, 256.0f);

            _UboVS.viewMatrix = Matrix4x4.CreateTranslation(0, 0, zoom);

            _UboVS.modelMatrix = Matrix4x4.Identity;
            _UboVS.modelMatrix = _UboVS.modelMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X);
            _UboVS.modelMatrix = _UboVS.modelMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y);
            _UboVS.modelMatrix = _UboVS.modelMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z);

            // Map uniform buffer and update it
            byte* pData;
            Util.CheckResult(vkMapMemory(device, _UniformBufferVS.memory, 0, (uint)sizeof(UboVS), 0, (void**)&pData));
            var uboVs = _UboVS;
            Unsafe.CopyBlock(pData, &uboVs, (uint)sizeof(UboVS));
            // Unmap after data has been copied
            // Note: Since we requested a host coherent memory type for the uniform buffer, the write is instantly visible to the GPU
            vkUnmapMemory(device, _UniformBufferVS.memory);
        }

        // Build separate command buffers for every framebuffer image
        // Unlike in OpenGL all rendering commands are recorded once into command buffers that are then resubmitted to the queue
        // This allows to generate work upfront and from multiple threads, one of the biggest advantages of Vulkan
        protected override void buildCommandBuffers()
        {
            VkCommandBufferBeginInfo cmdBufInfo = VkCommandBufferBeginInfo.New();

            // Set clear values for all framebuffer attachments with loadOp set to clear
            // We use two attachments (color and depth) that are cleared at the start of the subpass and as such we need to set clear values for both
            byte* clearValuesData = stackalloc byte[2 * sizeof(VkClearValue)];
            VkClearValue* clearValues = (VkClearValue*)clearValuesData;
            clearValues[0].color = new VkClearColorValue(0.0f, 0.0f, 0.2f);
            clearValues[1].depthStencil = new VkClearDepthStencilValue() { depth = 1.0f, stencil = 0 };

            VkRenderPassBeginInfo renderPassBeginInfo = VkRenderPassBeginInfo.New();
            renderPassBeginInfo.renderPass = renderPass;
            renderPassBeginInfo.renderArea.offset.x = 0;
            renderPassBeginInfo.renderArea.offset.y = 0;
            renderPassBeginInfo.renderArea.extent.width = width;
            renderPassBeginInfo.renderArea.extent.height = height;
            renderPassBeginInfo.clearValueCount = 2;
            renderPassBeginInfo.pClearValues = clearValues;

            for (int i = 0; i < drawCmdBuffers.Count; ++i)
            {
                // Set target frame buffer
                renderPassBeginInfo.framebuffer = frameBuffers[i];

                Util.CheckResult(vkBeginCommandBuffer(drawCmdBuffers[i], ref cmdBufInfo));

                // Start the first sub pass specified in our default render pass setup by the base class
                // This will clear the color and depth attachment
                vkCmdBeginRenderPass(drawCmdBuffers[i], ref renderPassBeginInfo, VkSubpassContents.Inline);

                // Update dynamic viewport state
                VkViewport viewport = new VkViewport();
                viewport.height = (float)height;
                viewport.width = (float)width;
                viewport.minDepth = (float)0.0f;
                viewport.maxDepth = (float)1.0f;
                vkCmdSetViewport(drawCmdBuffers[i], 0, 1, &viewport);

                // Update dynamic scissor state
                VkRect2D scissor = new VkRect2D();
                scissor.extent.width = width;
                scissor.extent.height = height;
                scissor.offset.x = 0;
                scissor.offset.y = 0;
                vkCmdSetScissor(drawCmdBuffers[i], 0, 1, &scissor);

                // Bind descriptor sets describing shader binding points
                var ds = DescriptorSet;
                vkCmdBindDescriptorSets(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, PipelineLayout, 0, 1, ref ds, 0, null);

                // Bind the rendering Pipeline
                // The Pipeline (state object) contains all states of the rendering Pipeline, binding it will set all the states specified at Pipeline creation time
                vkCmdBindPipeline(drawCmdBuffers[i], VkPipelineBindPoint.Graphics, Pipeline);

                // Bind triangle vertex buffer (contains position and colors)
                ulong offsets = 0;
                vkCmdBindVertexBuffers(drawCmdBuffers[i], 0, 1, ref _Vertices.buffer, ref offsets);

                // Bind triangle index buffer
                vkCmdBindIndexBuffer(drawCmdBuffers[i], _Indices.buffer, 0, VkIndexType.Uint32);

                // Draw indexed triangle
                vkCmdDrawIndexed(drawCmdBuffers[i], _Indices.count, 1, 0, 0, 1);

                vkCmdEndRenderPass(drawCmdBuffers[i]);

                // Ending the render pass will add an implicit barrier transitioning the frame buffer color attachment to 
                // VK_IMAGE_LAYOUT_PRESENT_SRC_KHR for presenting it to the windowing system

                Util.CheckResult(vkEndCommandBuffer(drawCmdBuffers[i]));
            }
        }

        void Draw()
        {
            // Get next image in the swap chain (back/front buffer)
            Util.CheckResult(Swapchain.AcquireNextImage(PresentCompleteSemaphore, ref currentBuffer));

            // Use a fence to wait until the command buffer has finished execution before using it again
            Util.CheckResult(vkWaitForFences(device, 1, ref waitFences[currentBuffer], True, ulong.MaxValue));
            Util.CheckResult(vkResetFences(device, 1, ref waitFences[currentBuffer]));

            // Pipeline stage at which the queue submission will wait (via pWaitSemaphores)
            VkPipelineStageFlags waitStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            // The submit info structure specifices a command buffer queue submission batch
            VkSubmitInfo submitInfo = VkSubmitInfo.New();
            submitInfo.pWaitDstStageMask = &waitStageMask;                                  // Pointer to the list of Pipeline stages that the semaphore waits will occur at
            var pcs = PresentCompleteSemaphore;
            submitInfo.pWaitSemaphores = &pcs;                         // Semaphore(s) to wait upon before the submitted command buffer starts executing
            submitInfo.waitSemaphoreCount = 1;                                              // One wait semaphore
            var rcs = RenderCompleteSemaphore;
            submitInfo.pSignalSemaphores = &rcs;                        // Semaphore(s) to be signaled when command buffers have completed
            submitInfo.signalSemaphoreCount = 1;                                            // One signal semaphore
            submitInfo.pCommandBuffers = (VkCommandBuffer*)drawCmdBuffers.GetAddress(currentBuffer);                    // Command buffers(s) to execute in this batch (submission)
            submitInfo.commandBufferCount = 1;                                              // One command buffer

            // Submit to the graphics queue passing a wait fence
            Util.CheckResult(vkQueueSubmit(queue, 1, ref submitInfo, waitFences[currentBuffer]));

            // Present the current buffer to the swap chain
            // Pass the semaphore signaled by the command buffer submission from the submit info as the wait semaphore for swap chain presentation
            // This ensures that the image is not presented to the windowing system until all commands have been submitted
            Util.CheckResult(Swapchain.QueuePresent(queue, currentBuffer, RenderCompleteSemaphore));
        }

        void SetupDescriptorPool()
        {
            // We need to tell the API the number of max. requested descriptors per type
            VkDescriptorPoolSize typeCount;
            // This example only uses one descriptor type (uniform buffer) and only requests one descriptor of this type
            typeCount.type = VkDescriptorType.UniformBuffer;
            typeCount.descriptorCount = 1;
            // For additional types you need to add new entries in the type count list
            // E.g. for two combined image samplers :
            // typeCounts[1].type = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
            // typeCounts[1].descriptorCount = 2;

            // Create the global descriptor pool
            // All descriptors used in this example are allocated from this pool
            VkDescriptorPoolCreateInfo descriptorPoolInfo = VkDescriptorPoolCreateInfo.New();
            descriptorPoolInfo.poolSizeCount = 1;
            descriptorPoolInfo.pPoolSizes = &typeCount;
            // Set the max. number of descriptor sets that can be requested from this pool (requesting beyond this limit will result in an error)
            descriptorPoolInfo.maxSets = 1;

            VkDescriptorPool descriptorPool;
            Util.CheckResult(vkCreateDescriptorPool(device, ref descriptorPoolInfo, null, out descriptorPool));
            DescriptorPool = descriptorPool;
        }

        void SetupDescriptorSetLayout()
        {
            // Setup layout of descriptors used in this example
            // Basically connects the different shader stages to descriptors for binding uniform buffers, image samplers, etc.
            // So every shader binding should map to one descriptor set layout binding

            // Binding 0: Uniform buffer (Vertex shader)
            VkDescriptorSetLayoutBinding layoutBinding = new VkDescriptorSetLayoutBinding();
            layoutBinding.descriptorType = VkDescriptorType.UniformBuffer;
            layoutBinding.descriptorCount = 1;
            layoutBinding.stageFlags = VkShaderStageFlags.Vertex;
            layoutBinding.pImmutableSamplers = null;

            VkDescriptorSetLayoutCreateInfo descriptorLayout = VkDescriptorSetLayoutCreateInfo.New();
            descriptorLayout.bindingCount = 1;
            descriptorLayout.pBindings = &layoutBinding;

            Util.CheckResult(vkCreateDescriptorSetLayout(device, ref descriptorLayout, null, out _descriptorSetLayout));

            // Create the Pipeline layout that is used to generate the rendering pipelines that are based on this descriptor set layout
            // In a more complex scenario you would have different Pipeline layouts for different descriptor set layouts that could be reused
            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo();
            pPipelineLayoutCreateInfo.sType = VkStructureType.PipelineLayoutCreateInfo;
            pPipelineLayoutCreateInfo.pNext = null;
            pPipelineLayoutCreateInfo.setLayoutCount = 1;
            VkDescriptorSetLayout dsl = _descriptorSetLayout;
            pPipelineLayoutCreateInfo.pSetLayouts = &dsl;

            Util.CheckResult(vkCreatePipelineLayout(device, &pPipelineLayoutCreateInfo, null, out _pipelineLayout));
        }

        void SetupDescriptorSet()
        {
            // Allocate a new descriptor set from the global descriptor pool
            VkDescriptorSetAllocateInfo allocInfo = VkDescriptorSetAllocateInfo.New();
            allocInfo.descriptorPool = DescriptorPool;
            allocInfo.descriptorSetCount = 1;
            VkDescriptorSetLayout dsl = DescriptorSetLayout;
            allocInfo.pSetLayouts = &dsl;

            Util.CheckResult(vkAllocateDescriptorSets(device, &allocInfo, out _descriptorSet));
            VkDescriptorSet descriptorSet = _descriptorSet;

            // Update the descriptor set determining the shader binding points
            // For every binding point used in a shader there needs to be one
            // descriptor set matching that binding point

            VkWriteDescriptorSet writeDescriptorSet = VkWriteDescriptorSet.New();

            // Binding 0 : Uniform buffer
            writeDescriptorSet.dstSet = descriptorSet;
            writeDescriptorSet.descriptorCount = 1;
            writeDescriptorSet.descriptorType = VkDescriptorType.UniformBuffer;
            var descriptor = _UniformBufferVS.descriptor;
            writeDescriptorSet.pBufferInfo = &descriptor;
            // Binds this uniform buffer to binding point 0
            writeDescriptorSet.dstBinding = 0;

            vkUpdateDescriptorSets(device, 1, ref writeDescriptorSet, 0, null);
        }

        // Create the depth (and stencil) buffer attachments used by our framebuffers
        // Note: Override of virtual function in the base class and called from within VulkanExampleBase::prepare
        protected override void SetupDepthStencil()
        {
            // Create an optimal image used as the depth stencil attachment
            VkImageCreateInfo image = VkImageCreateInfo.New();
            image.sType = VkStructureType.ImageCreateInfo;
            image.imageType = VkImageType.Image2D;
            image.format = DepthFormat;
            // Use example's height and width
            image.extent = new VkExtent3D() { width = width, height = height, depth = 1 };
            image.mipLevels = 1;
            image.arrayLayers = 1;
            image.samples = VkSampleCountFlags.Count1;
            image.tiling = VkImageTiling.Optimal;
            image.usage = (VkImageUsageFlags.DepthStencilAttachment | VkImageUsageFlags.TransferSrc);
            image.initialLayout = VkImageLayout.Undefined;
            Util.CheckResult(vkCreateImage(device, &image, null, out DepthStencil.Image));

            // Allocate memory for the image (Device local) and bind it to our image
            VkMemoryAllocateInfo memAlloc = VkMemoryAllocateInfo.New();
            vkGetImageMemoryRequirements(device, DepthStencil.Image, out VkMemoryRequirements memReqs);
            memAlloc.allocationSize = memReqs.size;
            memAlloc.memoryTypeIndex = GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);
            Util.CheckResult(vkAllocateMemory(device, ref memAlloc, null, out VkDeviceMemory memory));
            DepthStencil.Mem = memory;
            Util.CheckResult(vkBindImageMemory(device, DepthStencil.Image, DepthStencil.Mem, 0));

            // Create a view for the depth stencil image
            // Images aren't directly accessed in Vulkan, but rather through views described by a subresource range
            // This allows for multiple views of one image with differing ranges (e.g. for different layers)
            VkImageViewCreateInfo depthStencilView = VkImageViewCreateInfo.New();
            depthStencilView.viewType = VkImageViewType.Image2D;
            depthStencilView.format = DepthFormat;
            depthStencilView.subresourceRange = new VkImageSubresourceRange();
            depthStencilView.subresourceRange.aspectMask = (VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil);
            depthStencilView.subresourceRange.baseMipLevel = 0;
            depthStencilView.subresourceRange.levelCount = 1;
            depthStencilView.subresourceRange.baseArrayLayer = 0;
            depthStencilView.subresourceRange.layerCount = 1;
            depthStencilView.image = DepthStencil.Image;
            Util.CheckResult(vkCreateImageView(device, &depthStencilView, null, out DepthStencil.View));
        }

        // Create a frame buffer for each swap chain image
        // Note: Override of virtual function in the base class and called from within VulkanExampleBase::prepare
        protected override void SetupFrameBuffer()
        {
            // Create a frame buffer for every image in the swapchain
            frameBuffers.Resize(Swapchain.ImageCount);
            frameBuffers.Count = Swapchain.ImageCount;
            for (uint i = 0; i < frameBuffers.Count; i++)
            {
                byte* attachmentData = stackalloc byte[2 * sizeof(VkImageView)];
                VkImageView* attachments = (VkImageView*)attachmentData;
                attachments[0] = Swapchain.Buffers[(uint)i].View;                                 // Color attachment is the view of the swapchain image			
                attachments[1] = DepthStencil.View;                                         // Depth/Stencil attachment is the same for all frame buffers			

                VkFramebufferCreateInfo frameBufferCreateInfo = new VkFramebufferCreateInfo();
                frameBufferCreateInfo.sType = VkStructureType.FramebufferCreateInfo;
                // All frame buffers use the same renderpass setup
                frameBufferCreateInfo.renderPass = renderPass;
                frameBufferCreateInfo.attachmentCount = 2;
                frameBufferCreateInfo.pAttachments = attachments;
                frameBufferCreateInfo.width = width;
                frameBufferCreateInfo.height = height;
                frameBufferCreateInfo.layers = 1;
                // Create the framebuffer
                Util.CheckResult(vkCreateFramebuffer(device, &frameBufferCreateInfo, null, (VkFramebuffer*)frameBuffers.GetAddress(i)));
            }
        }

        // Render pass setup
        // Render passes are a new concept in Vulkan. They describe the attachments used during rendering and may contain multiple subpasses with attachment dependencies 
        // This allows the driver to know up-front what the rendering will look like and is a good opportunity to optimize especially on tile-based renderers (with multiple subpasses)
        // Using sub pass dependencies also adds implicit layout transitions for the attachment used, so we don't need to add explicit image memory barriers to transform them
        // Note: Override of virtual function in the base class and called from within VulkanExampleBase::prepare
        protected override void SetupRenderPass()
        {
            // This example will use a single render pass with one subpass

            // Descriptors for the attachments used by this renderpass
            FixedArray2<VkAttachmentDescription> attachments = new FixedArray2<VkAttachmentDescription>();

            // Color attachment
            attachments.First.format = Swapchain.ColorFormat;                                  // Use the color format selected by the swapchain
            attachments.First.samples = VkSampleCountFlags.Count1;                              // We don't use multi sampling in this example
            attachments.First.loadOp = VkAttachmentLoadOp.Clear;                               // Clear this attachment at the start of the render pass
            attachments.First.storeOp = VkAttachmentStoreOp.Store;                             // Keep it's contents after the render pass is finished (for displaying it)
            attachments.First.stencilLoadOp = VkAttachmentLoadOp.DontCare;                     // We don't use stencil, so don't care for load
            attachments.First.stencilStoreOp = VkAttachmentStoreOp.DontCare;                   // Same for store
            attachments.First.initialLayout = VkImageLayout.Undefined;                         // Layout at render pass start. Initial doesn't matter, so we use undefined
            attachments.First.finalLayout = VkImageLayout.PresentSrcKHR;                          // Layout to which the attachment is transitioned when the render pass is finished
                                                                                               // As we want to present the color buffer to the swapchain, we transition to PRESENT_KHR	
                                                                                               // Depth attachment
            attachments.Second.format = DepthFormat;                                            // A proper depth format is selected in the example base
            attachments.Second.samples = VkSampleCountFlags.Count1;
            attachments.Second.loadOp = VkAttachmentLoadOp.Clear;                            // Clear depth at start of first subpass
            attachments.Second.storeOp = VkAttachmentStoreOp.DontCare;                      // We don't need depth after render pass has finished (DONT_CARE may result in better performance)
            attachments.Second.stencilLoadOp = VkAttachmentLoadOp.DontCare;                 // No stencil
            attachments.Second.stencilStoreOp = VkAttachmentStoreOp.DontCare;               // No Stencil
            attachments.Second.initialLayout = VkImageLayout.Undefined;                       // Layout at render pass start. Initial doesn't matter, so we use undefined
            attachments.Second.finalLayout = VkImageLayout.ColorAttachmentOptimal;  // Transition to depth/stencil attachment

            // Setup attachment references
            VkAttachmentReference colorReference = new VkAttachmentReference();
            colorReference.attachment = 0;                                                  // Attachment 0 is color
            colorReference.layout = VkImageLayout.ColorAttachmentOptimal;               // Attachment layout used as color during the subpass

            VkAttachmentReference depthReference = new VkAttachmentReference();
            depthReference.attachment = 1;                                                  // Attachment 1 is color
            depthReference.layout = VkImageLayout.DepthStencilAttachmentOptimal;       // Attachment used as depth/stemcil used during the subpass

            // Setup a single subpass reference
            VkSubpassDescription subpassDescription = new VkSubpassDescription();
            subpassDescription.pipelineBindPoint = VkPipelineBindPoint.Graphics;
            subpassDescription.colorAttachmentCount = 1;                                    // Subpass uses one color attachment
            subpassDescription.pColorAttachments = &colorReference;                         // Reference to the color attachment in slot 0
            subpassDescription.pDepthStencilAttachment = &depthReference;                   // Reference to the depth attachment in slot 1
            subpassDescription.inputAttachmentCount = 0;                                    // Input attachments can be used to sample from contents of a previous subpass
            subpassDescription.pInputAttachments = null;                                 // (Input attachments not used by this example)
            subpassDescription.preserveAttachmentCount = 0;                                 // Preserved attachments can be used to loop (and preserve) attachments through subpasses
            subpassDescription.pPreserveAttachments = null;                              // (Preserve attachments not used by this example)
            subpassDescription.pResolveAttachments = null;                               // Resolve attachments are resolved at the end of a sub pass and can be used for e.g. multi sampling

            // Setup subpass dependencies
            // These will add the implicit ttachment layout transitionss specified by the attachment descriptions
            // The actual usage layout is preserved through the layout specified in the attachment reference		
            // Each subpass dependency will introduce a memory and execution dependency between the source and dest subpass described by
            // srcStageMask, dstStageMask, srcAccessMask, dstAccessMask (and dependencyFlags is set)
            // Note: VK_SUBPASS_EXTERNAL is a special constant that refers to all commands executed outside of the actual renderpass)
            FixedArray2<VkSubpassDependency> dependencies = new FixedArray2<VkSubpassDependency>();

            // First dependency at the start of the renderpass
            // Does the transition from final to initial layout 
            dependencies.First.srcSubpass = SubpassExternal;                               // Producer of the dependency 
            dependencies.First.dstSubpass = 0;                                                 // Consumer is our single subpass that will wait for the execution depdendency
            dependencies.First.srcStageMask = VkPipelineStageFlags.BottomOfPipe;
            dependencies.First.dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            dependencies.First.srcAccessMask = VkAccessFlags.MemoryRead;
            dependencies.First.dstAccessMask = (VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite);
            dependencies.First.dependencyFlags = (VkDependencyFlags.ByRegion);

            // Second dependency at the end the renderpass
            // Does the transition from the initial to the final layout
            dependencies.Second.srcSubpass = 0;                                                 // Producer of the dependency is our single subpass
            dependencies.Second.dstSubpass = SubpassExternal;                               // Consumer are all commands outside of the renderpass
            dependencies.Second.srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            dependencies.Second.dstStageMask = VkPipelineStageFlags.BottomOfPipe;
            dependencies.Second.srcAccessMask = (VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite);
            dependencies.Second.dstAccessMask = VkAccessFlags.MemoryRead;
            dependencies.Second.dependencyFlags = VkDependencyFlags.ByRegion;

            // Create the actual renderpass
            VkRenderPassCreateInfo renderPassInfo = VkRenderPassCreateInfo.New();
            renderPassInfo.attachmentCount = 2;                                             // Number of attachments used by this render pass
            renderPassInfo.pAttachments = (VkAttachmentDescription*)Unsafe.AsPointer(ref attachments);                                      // Descriptions of the attachments used by the render pass
            renderPassInfo.subpassCount = 1;                                                // We only use one subpass in this example
            renderPassInfo.pSubpasses = &subpassDescription;                                // Description of that subpass
            renderPassInfo.dependencyCount = 2;                                             // Number of subpass dependencies
            renderPassInfo.pDependencies = (VkSubpassDependency*)Unsafe.AsPointer(ref dependencies);                                    // Subpass dependencies used by the render pass

            Util.CheckResult(vkCreateRenderPass(device, ref renderPassInfo, null, out _renderPass));
        }

        // Vulkan loads it's shaders from an immediate binary representation called SPIR-V
        // Shaders are compiled offline from e.g. GLSL using the reference glslang compiler
        // This function loads such a shader from a binary file and returns a shader module structure
        VkShaderModule LoadSPIRVShader(string filename)
        {
            byte[] shaderCode = File.ReadAllBytes(filename);
            ulong shaderSize = (ulong)shaderCode.Length;
            fixed (byte* scPtr = shaderCode)
            {
                // Create a new shader module that will be used for Pipeline creation
                VkShaderModuleCreateInfo moduleCreateInfo = VkShaderModuleCreateInfo.New();
                moduleCreateInfo.codeSize = new UIntPtr(shaderSize);
                moduleCreateInfo.pCode = (uint*)scPtr;

                Util.CheckResult(vkCreateShaderModule(device, ref moduleCreateInfo, null, out VkShaderModule shaderModule));

                return shaderModule;
            }
        }

        void preparePipelines()
        {
            // Create the graphics Pipeline used in this example
            // Vulkan uses the concept of rendering pipelines to encapsulate fixed states, replacing OpenGL's complex state machine
            // A Pipeline is then stored and hashed on the GPU making Pipeline changes very fast
            // Note: There are still a few dynamic states that are not directly part of the Pipeline (but the info that they are used is)

            VkGraphicsPipelineCreateInfo pipelineCreateInfo = VkGraphicsPipelineCreateInfo.New();
            // The layout used for this Pipeline (can be shared among multiple pipelines using the same layout)
            pipelineCreateInfo.layout = PipelineLayout;
            // Renderpass this Pipeline is attached to
            pipelineCreateInfo.renderPass = renderPass;

            // Construct the differnent states making up the Pipeline

            // Input assembly state describes how primitives are assembled
            // This Pipeline will assemble vertex data as a triangle lists (though we only use one triangle)
            VkPipelineInputAssemblyStateCreateInfo inputAssemblyState = VkPipelineInputAssemblyStateCreateInfo.New();
            inputAssemblyState.topology = VkPrimitiveTopology.TriangleList;

            // Rasterization state
            VkPipelineRasterizationStateCreateInfo rasterizationState = VkPipelineRasterizationStateCreateInfo.New();
            rasterizationState.polygonMode = VkPolygonMode.Fill;
            rasterizationState.cullMode = (uint)VkCullModeFlags.None;
            rasterizationState.frontFace = VkFrontFace.CounterClockwise;
            rasterizationState.depthClampEnable = False;
            rasterizationState.rasterizerDiscardEnable = False;
            rasterizationState.depthBiasEnable = False;
            rasterizationState.lineWidth = 1.0f;

            // Color blend state describes how blend factors are calculated (if used)
            // We need one blend attachment state per color attachment (even if blending is not used
            VkPipelineColorBlendAttachmentState blendAttachmentState = new VkPipelineColorBlendAttachmentState();
            blendAttachmentState.colorWriteMask = VkColorComponentFlags.R | VkColorComponentFlags.G | VkColorComponentFlags.B | VkColorComponentFlags.A;
            blendAttachmentState.blendEnable = False;
            VkPipelineColorBlendStateCreateInfo colorBlendState = VkPipelineColorBlendStateCreateInfo.New();
            colorBlendState.attachmentCount = 1;
            colorBlendState.pAttachments = &blendAttachmentState;

            // Viewport state sets the number of viewports and scissor used in this Pipeline
            // Note: This is actually overriden by the dynamic states (see below)
            VkPipelineViewportStateCreateInfo viewportState = VkPipelineViewportStateCreateInfo.New();
            viewportState.viewportCount = 1;
            viewportState.scissorCount = 1;

            // Enable dynamic states
            // Most states are baked into the Pipeline, but there are still a few dynamic states that can be changed within a command buffer
            // To be able to change these we need do specify which dynamic states will be changed using this Pipeline. Their actual states are set later on in the command buffer.
            // For this example we will set the viewport and scissor using dynamic states
            FixedArray2<VkDynamicState> dynamicStateEnables
                = new FixedArray2<VkDynamicState>(VkDynamicState.Viewport, VkDynamicState.Scissor);
            VkPipelineDynamicStateCreateInfo dynamicState = VkPipelineDynamicStateCreateInfo.New();
            dynamicState.pDynamicStates = (VkDynamicState*)Unsafe.AsPointer(ref dynamicStateEnables);
            dynamicState.dynamicStateCount = dynamicStateEnables.Count;

            // Depth and stencil state containing depth and stencil compare and test operations
            // We only use depth tests and want depth tests and writes to be enabled and compare with less or equal
            VkPipelineDepthStencilStateCreateInfo depthStencilState = VkPipelineDepthStencilStateCreateInfo.New();
            depthStencilState.depthTestEnable = True;
            depthStencilState.depthWriteEnable = True;
            depthStencilState.depthCompareOp = VkCompareOp.LessOrEqual;
            depthStencilState.depthBoundsTestEnable = False;
            depthStencilState.back.failOp = VkStencilOp.Keep;
            depthStencilState.back.passOp = VkStencilOp.Keep;
            depthStencilState.back.compareOp = VkCompareOp.Always;
            depthStencilState.stencilTestEnable = False;
            depthStencilState.front = depthStencilState.back;

            // Multi sampling state
            // This example does not make use fo multi sampling (for anti-aliasing), the state must still be set and passed to the Pipeline
            VkPipelineMultisampleStateCreateInfo multisampleState = VkPipelineMultisampleStateCreateInfo.New();
            multisampleState.rasterizationSamples = VkSampleCountFlags.Count1;
            multisampleState.pSampleMask = null;

            // Vertex input descriptions 
            // Specifies the vertex input parameters for a Pipeline

            // Vertex input binding
            // This example uses a single vertex input binding at binding point 0 (see vkCmdBindVertexBuffers)
            VkVertexInputBindingDescription vertexInputBinding = new VkVertexInputBindingDescription();
            vertexInputBinding.binding = 0;
            vertexInputBinding.stride = (uint)sizeof(Vertex);
            vertexInputBinding.inputRate = VkVertexInputRate.Vertex;

            // Inpute attribute bindings describe shader attribute locations and memory layouts
            byte* viaData = stackalloc byte[2 * sizeof(VkVertexInputAttributeDescription)];
            VkVertexInputAttributeDescription* vertexInputAttributs = (VkVertexInputAttributeDescription*)viaData;
            // These match the following shader layout (see triangle.vert):
            //	layout (location = 0) in vec3 inPos;
            //	layout (location = 1) in vec3 inColor;
            // Attribute location 0: Position
            vertexInputAttributs[0].binding = 0;
            vertexInputAttributs[0].location = 0;
            // Position attribute is three 32 bit signed (SFLOAT) floats (R32 G32 B32)
            vertexInputAttributs[0].format = VkFormat.R32g32b32Sfloat;
            vertexInputAttributs[0].offset = 0;
            // Attribute location 1: Color
            vertexInputAttributs[1].binding = 0;
            vertexInputAttributs[1].location = 1;
            // Color attribute is three 32 bit signed (SFLOAT) floats (R32 G32 B32)
            vertexInputAttributs[1].format = VkFormat.R32g32b32Sfloat;
            vertexInputAttributs[1].offset = 12;

            // Vertex input state used for Pipeline creation
            VkPipelineVertexInputStateCreateInfo vertexInputState = VkPipelineVertexInputStateCreateInfo.New();
            vertexInputState.vertexBindingDescriptionCount = 1;
            vertexInputState.pVertexBindingDescriptions = &vertexInputBinding;
            vertexInputState.vertexAttributeDescriptionCount = 2;
            vertexInputState.pVertexAttributeDescriptions = vertexInputAttributs;

            // Shaders
            VkPipelineShaderStageCreateInfo[] shaderStages = new VkPipelineShaderStageCreateInfo[2];
            // Vertex shader
            shaderStages[0].sType = VkStructureType.PipelineShaderStageCreateInfo;
            // Set Pipeline stage for this shader
            shaderStages[0].stage = VkShaderStageFlags.Vertex;
            // Load binary SPIR-V shader
            shaderStages[0].module = LoadSPIRVShader(Path.Combine(AppContext.BaseDirectory, "Shaders/triangle.vert.spv"));
            // Main entry point for the shader
            byte[] mainBytes = Encoding.UTF8.GetBytes("main");
            fixed (byte* mainBytesPtr = mainBytes)
            {
                shaderStages[0].pName = mainBytesPtr;
                Debug.Assert(shaderStages[0].module != 0);

                // Fragment shader
                shaderStages[1].sType = VkStructureType.PipelineShaderStageCreateInfo;
                // Set Pipeline stage for this shader
                shaderStages[1].stage = VkShaderStageFlags.Fragment;
                // Load binary SPIR-V shader
                shaderStages[1].module = LoadSPIRVShader(Path.Combine(AppContext.BaseDirectory, "Shaders/triangle.frag.spv"));
                // Main entry point for the shader
                shaderStages[1].pName = mainBytesPtr;
                Debug.Assert(shaderStages[1].module != 0);
            }

            fixed (VkPipelineShaderStageCreateInfo* ssPtr = shaderStages)
            {
                // Set Pipeline shader stage info
                pipelineCreateInfo.stageCount = (uint)shaderStages.Length;
                pipelineCreateInfo.pStages = ssPtr;

                // Assign the Pipeline states to the Pipeline creation info structure
                pipelineCreateInfo.pVertexInputState = &vertexInputState;
                pipelineCreateInfo.pInputAssemblyState = &inputAssemblyState;
                pipelineCreateInfo.pRasterizationState = &rasterizationState;
                pipelineCreateInfo.pColorBlendState = &colorBlendState;
                pipelineCreateInfo.pMultisampleState = &multisampleState;
                pipelineCreateInfo.pViewportState = &viewportState;
                pipelineCreateInfo.pDepthStencilState = &depthStencilState;
                pipelineCreateInfo.renderPass = renderPass;
                pipelineCreateInfo.pDynamicState = &dynamicState;

                // Create rendering Pipeline using the specified states
                Util.CheckResult(vkCreateGraphicsPipelines(device, pipelineCache, 1, ref pipelineCreateInfo, null, out _pipeline));

                // Shader modules are no longer needed once the graphics Pipeline has been created
                vkDestroyShaderModule(device, shaderStages[0].module, null);
                vkDestroyShaderModule(device, shaderStages[1].module, null);
            }
        }

        protected override void render()
        {
            if (prepared)
            {
                Draw();
            }
        }

        protected override void viewChanged()
        {
            UpdateUniformBuffers();
        }
    }

    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Color;
    }

    struct StagingBuffer
    {
        public VkDeviceMemory memory;
        public VkBuffer buffer;
    };

    struct StagingBuffers
    {

        public StagingBuffer vertices;
        public StagingBuffer indices;
    }

    // Vertex buffer and attributes
    public struct Vertices
    {

        public VkDeviceMemory memory;      // Handle to the Device memory for this buffer
        public VkBuffer buffer;            // Handle to the Vulkan buffer object that the memory is bound to
    }

    // Index buffer
    public struct Indices
    {
        public VkDeviceMemory memory;
        public VkBuffer buffer;
        public uint count;
    }

    // Uniform buffer block object
    public struct UniformBufferVS
    {
        public VkDeviceMemory memory;
        public VkBuffer buffer;
        public VkDescriptorBufferInfo descriptor;
    }

    // For simplicity we use the same uniform block layout as in the shader:
    //
    //	layout(set = 0, binding = 0) uniform UBO
    //	{
    //		mat4 projectionMatrix;
    //		mat4 modelMatrix;
    //		mat4 viewMatrix;
    //	} ubo;
    //
    // This way we can just memcopy the ubo data to the ubo
    // Note: You should use data types that align with the GPU in order to avoid manual padding (vec4, mat4)
    public struct UboVS
    {
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 modelMatrix;
        public Matrix4x4 viewMatrix;
    }

    public struct TriangleIndices
    {
        public int Index0;
        public int Index1;
        public int Index2;
    }
}
