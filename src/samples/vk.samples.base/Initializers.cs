using System;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe static class Initializers
    {
        public static VkSemaphoreCreateInfo SemaphoreCreateInfo()
        {
            VkSemaphoreCreateInfo semaphoreCreateInfo = new VkSemaphoreCreateInfo();
            semaphoreCreateInfo.sType = VkStructureType.SemaphoreCreateInfo;
            return semaphoreCreateInfo;
        }

        public static VkSubmitInfo SubmitInfo()
        {
            VkSubmitInfo submitInfo = new VkSubmitInfo();
            submitInfo.sType = VkStructureType.SubmitInfo;
            return submitInfo;
        }

        public static VkCommandBufferAllocateInfo CommandBufferAllocateInfo(
            VkCommandPool commandPool,
            VkCommandBufferLevel level,
            uint bufferCount)
        {
            VkCommandBufferAllocateInfo commandBufferAllocateInfo = new VkCommandBufferAllocateInfo();
            commandBufferAllocateInfo.sType = VkStructureType.CommandBufferAllocateInfo;
            commandBufferAllocateInfo.commandPool = commandPool;
            commandBufferAllocateInfo.level = level;
            commandBufferAllocateInfo.commandBufferCount = bufferCount;
            return commandBufferAllocateInfo;
        }

        public static VkCommandBufferBeginInfo CommandBufferBeginInfo()
        {
            VkCommandBufferBeginInfo cmdBufferBeginInfo = new VkCommandBufferBeginInfo();
            cmdBufferBeginInfo.sType = VkStructureType.CommandBufferBeginInfo;
            return cmdBufferBeginInfo;
        }

        public static VkPipelineInputAssemblyStateCreateInfo pipelineInputAssemblyStateCreateInfo(
            VkPrimitiveTopology topology,
            uint flags,
            uint primitiveRestartEnable)
        {
            VkPipelineInputAssemblyStateCreateInfo pipelineInputAssemblyStateCreateInfo = VkPipelineInputAssemblyStateCreateInfo.New();
            pipelineInputAssemblyStateCreateInfo.topology = topology;
            pipelineInputAssemblyStateCreateInfo.flags = flags;
            pipelineInputAssemblyStateCreateInfo.primitiveRestartEnable = primitiveRestartEnable;
            return pipelineInputAssemblyStateCreateInfo;
        }

        public static VkPipelineRasterizationStateCreateInfo pipelineRasterizationStateCreateInfo(
            VkPolygonMode polygonMode,
            VkCullModeFlags cullMode,
            VkFrontFace frontFace,
            uint flags = 0)
        {
            VkPipelineRasterizationStateCreateInfo pipelineRasterizationStateCreateInfo = VkPipelineRasterizationStateCreateInfo.New();
            pipelineRasterizationStateCreateInfo.polygonMode = polygonMode;
            pipelineRasterizationStateCreateInfo.cullMode = cullMode;
            pipelineRasterizationStateCreateInfo.frontFace = frontFace;
            pipelineRasterizationStateCreateInfo.flags = flags;
            pipelineRasterizationStateCreateInfo.depthClampEnable = False;
            pipelineRasterizationStateCreateInfo.lineWidth = 1.0f;
            return pipelineRasterizationStateCreateInfo;
        }

        public static VkPipelineColorBlendAttachmentState pipelineColorBlendAttachmentState(
            VkColorComponentFlags colorWriteMask,
            uint blendEnable)
        {
            VkPipelineColorBlendAttachmentState pipelineColorBlendAttachmentState = new VkPipelineColorBlendAttachmentState();
            pipelineColorBlendAttachmentState.colorWriteMask = colorWriteMask;
            pipelineColorBlendAttachmentState.blendEnable = blendEnable;
            return pipelineColorBlendAttachmentState;
        }

        public static VkPipelineColorBlendStateCreateInfo pipelineColorBlendStateCreateInfo(
            uint attachmentCount,
             VkPipelineColorBlendAttachmentState* pAttachments)
        {

            VkPipelineColorBlendStateCreateInfo pipelineColorBlendStateCreateInfo = VkPipelineColorBlendStateCreateInfo.New();
            pipelineColorBlendStateCreateInfo.attachmentCount = attachmentCount;
            pipelineColorBlendStateCreateInfo.pAttachments = pAttachments;
            return pipelineColorBlendStateCreateInfo;
        }

        public static VkPipelineDepthStencilStateCreateInfo pipelineDepthStencilStateCreateInfo(
            uint depthTestEnable,
            uint depthWriteEnable,
            VkCompareOp depthCompareOp)
        {
            VkPipelineDepthStencilStateCreateInfo pipelineDepthStencilStateCreateInfo = VkPipelineDepthStencilStateCreateInfo.New();
            pipelineDepthStencilStateCreateInfo.depthTestEnable = depthTestEnable;
            pipelineDepthStencilStateCreateInfo.depthWriteEnable = depthWriteEnable;
            pipelineDepthStencilStateCreateInfo.depthCompareOp = depthCompareOp;
            pipelineDepthStencilStateCreateInfo.front = pipelineDepthStencilStateCreateInfo.back;
            pipelineDepthStencilStateCreateInfo.back.compareOp = VkCompareOp.Always;
            return pipelineDepthStencilStateCreateInfo;
        }

        public static VkPipelineViewportStateCreateInfo pipelineViewportStateCreateInfo(
            uint viewportCount,
            uint scissorCount,
            uint flags = 0)
        {
            VkPipelineViewportStateCreateInfo pipelineViewportStateCreateInfo = VkPipelineViewportStateCreateInfo.New();
            pipelineViewportStateCreateInfo.viewportCount = viewportCount;
            pipelineViewportStateCreateInfo.scissorCount = scissorCount;
            pipelineViewportStateCreateInfo.flags = flags;
            return pipelineViewportStateCreateInfo;
        }

        public static VkPipelineMultisampleStateCreateInfo pipelineMultisampleStateCreateInfo(
            VkSampleCountFlags rasterizationSamples,
            uint flags = 0)
        {
            VkPipelineMultisampleStateCreateInfo pipelineMultisampleStateCreateInfo = VkPipelineMultisampleStateCreateInfo.New();
            pipelineMultisampleStateCreateInfo.rasterizationSamples = rasterizationSamples;
            pipelineMultisampleStateCreateInfo.flags = flags;
            return pipelineMultisampleStateCreateInfo;
        }

        public static VkPipelineDynamicStateCreateInfo pipelineDynamicStateCreateInfo(
            VkDynamicState* pDynamicStates,
            uint dynamicStateCount,
            uint flags = 0)
        {
            VkPipelineDynamicStateCreateInfo pipelineDynamicStateCreateInfo = VkPipelineDynamicStateCreateInfo.New();
            pipelineDynamicStateCreateInfo.pDynamicStates = pDynamicStates;
            pipelineDynamicStateCreateInfo.dynamicStateCount = dynamicStateCount;
            pipelineDynamicStateCreateInfo.flags = flags;
            return pipelineDynamicStateCreateInfo;
        }

        public static VkPipelineDynamicStateCreateInfo pipelineDynamicStateCreateInfo(
            NativeList<VkDynamicState> pDynamicStates,
            uint flags = 0)
        {
            VkPipelineDynamicStateCreateInfo pipelineDynamicStateCreateInfo = VkPipelineDynamicStateCreateInfo.New();
            pipelineDynamicStateCreateInfo.pDynamicStates = (VkDynamicState*)pDynamicStates.Data;
            pipelineDynamicStateCreateInfo.dynamicStateCount = pDynamicStates.Count;
            pipelineDynamicStateCreateInfo.flags = flags;
            return pipelineDynamicStateCreateInfo;
        }

        public static VkPipelineTessellationStateCreateInfo pipelineTessellationStateCreateInfo(uint patchControlPoints)
        {
            VkPipelineTessellationStateCreateInfo pipelineTessellationStateCreateInfo = VkPipelineTessellationStateCreateInfo.New();
            pipelineTessellationStateCreateInfo.patchControlPoints = patchControlPoints;
            return pipelineTessellationStateCreateInfo;
        }

        public static VkGraphicsPipelineCreateInfo pipelineCreateInfo(
            VkPipelineLayout layout,
            VkRenderPass renderPass,
            VkPipelineCreateFlags flags = 0)
        {
            VkGraphicsPipelineCreateInfo pipelineCreateInfo = VkGraphicsPipelineCreateInfo.New();
            pipelineCreateInfo.layout = layout;
            pipelineCreateInfo.renderPass = renderPass;
            pipelineCreateInfo.flags = flags;
            pipelineCreateInfo.basePipelineIndex = -1;
            pipelineCreateInfo.basePipelineHandle = new VkPipeline();
            return pipelineCreateInfo;
        }

        public static VkVertexInputBindingDescription vertexInputBindingDescription(
            uint binding,
            uint stride,
            VkVertexInputRate inputRate)
        {
            VkVertexInputBindingDescription vInputBindDescription = new VkVertexInputBindingDescription();
            vInputBindDescription.binding = binding;
            vInputBindDescription.stride = stride;
            vInputBindDescription.inputRate = inputRate;
            return vInputBindDescription;
        }

        public static VkVertexInputAttributeDescription vertexInputAttributeDescription(
            uint binding,
            uint location,
            VkFormat format,
            uint offset)
        {
            VkVertexInputAttributeDescription vInputAttribDescription = new VkVertexInputAttributeDescription();
            vInputAttribDescription.location = location;
            vInputAttribDescription.binding = binding;
            vInputAttribDescription.format = format;
            vInputAttribDescription.offset = offset;
            return vInputAttribDescription;
        }

        public static VkWriteDescriptorSet writeDescriptorSet(
            VkDescriptorSet dstSet,
            VkDescriptorType type,
            uint binding,
            VkDescriptorBufferInfo* bufferInfo,
            uint descriptorCount = 1)
        {
            VkWriteDescriptorSet writeDescriptorSet = VkWriteDescriptorSet.New();
            writeDescriptorSet.dstSet = dstSet;
            writeDescriptorSet.descriptorType = type;
            writeDescriptorSet.dstBinding = binding;
            writeDescriptorSet.pBufferInfo = bufferInfo;
            writeDescriptorSet.descriptorCount = descriptorCount;
            return writeDescriptorSet;
        }

        /** @brief Initialize an image memory barrier with no image transfer ownership */
        public static VkImageMemoryBarrier imageMemoryBarrier()
        {
            VkImageMemoryBarrier imageMemoryBarrier = VkImageMemoryBarrier.New();
            imageMemoryBarrier.srcQueueFamilyIndex = QueueFamilyIgnored;
            imageMemoryBarrier.dstQueueFamilyIndex = QueueFamilyIgnored;
            return imageMemoryBarrier;
        }

        public static VkMemoryAllocateInfo memoryAllocateInfo()
        {
            VkMemoryAllocateInfo memAllocInfo = new VkMemoryAllocateInfo();
            memAllocInfo.sType =  VkStructureType.MemoryAllocateInfo;
            return memAllocInfo;
        }


        public static VkBufferCreateInfo bufferCreateInfo()
        {
            VkBufferCreateInfo bufCreateInfo = VkBufferCreateInfo.New();
            return bufCreateInfo;
        }
    }
}
