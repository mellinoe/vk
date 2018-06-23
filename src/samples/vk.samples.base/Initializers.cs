// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: base/VulkanInitializers.hpp, 

/*
* Vulkan Example base class
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe static class Initializers
    {
        public static VkSemaphoreCreateInfo semaphoreCreateInfo()
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

        public static VkCommandBufferBeginInfo commandBufferBeginInfo()
        {
            VkCommandBufferBeginInfo cmdBufferBeginInfo = new VkCommandBufferBeginInfo();
            cmdBufferBeginInfo.sType = VkStructureType.CommandBufferBeginInfo;
            return cmdBufferBeginInfo;
        }


        public static VkRenderPassBeginInfo renderPassBeginInfo()
        {
            VkRenderPassBeginInfo renderPassBeginInfo = VkRenderPassBeginInfo.New();
            return renderPassBeginInfo;
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

        public static VkPipelineColorBlendAttachmentState pipelineColorBlendAttachmentState(
            uint colorWriteMask,
            uint blendEnable)
        {
            VkPipelineColorBlendAttachmentState pipelineColorBlendAttachmentState = new VkPipelineColorBlendAttachmentState();
            pipelineColorBlendAttachmentState.colorWriteMask = (VkColorComponentFlags)colorWriteMask;
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

        public static VkWriteDescriptorSet writeDescriptorSet(
            VkDescriptorSet dstSet,
            VkDescriptorType type,
            uint binding,
            VkDescriptorImageInfo* imageInfo,
            uint descriptorCount = 1)
        {
            VkWriteDescriptorSet writeDescriptorSet = VkWriteDescriptorSet.New();
            writeDescriptorSet.dstSet = dstSet;
            writeDescriptorSet.descriptorType = type;
            writeDescriptorSet.dstBinding = binding;
            writeDescriptorSet.pImageInfo = imageInfo;
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

        public static VkImageCreateInfo imageCreateInfo()
        {
            VkImageCreateInfo imageCreateInfo = VkImageCreateInfo.New();
            return imageCreateInfo;
        }

        public static VkMemoryAllocateInfo memoryAllocateInfo()
        {
            VkMemoryAllocateInfo memAllocInfo = new VkMemoryAllocateInfo();
            memAllocInfo.sType = VkStructureType.MemoryAllocateInfo;
            return memAllocInfo;
        }


        public static VkBufferCreateInfo bufferCreateInfo()
        {
            VkBufferCreateInfo bufCreateInfo = VkBufferCreateInfo.New();
            return bufCreateInfo;
        }

        public static VkBufferCreateInfo bufferCreateInfo(
            VkBufferUsageFlags usage,
            ulong size)
        {
            VkBufferCreateInfo bufCreateInfo = VkBufferCreateInfo.New();
            bufCreateInfo.usage = usage;
            bufCreateInfo.size = size;
            return bufCreateInfo;
        }

        public static VkSamplerCreateInfo samplerCreateInfo()
        {
            VkSamplerCreateInfo samplerCreateInfo = VkSamplerCreateInfo.New();
            return samplerCreateInfo;
        }

        public static VkImageViewCreateInfo imageViewCreateInfo()
        {
            VkImageViewCreateInfo imageViewCreateInfo = VkImageViewCreateInfo.New();
            return imageViewCreateInfo;
        }

        public static VkViewport viewport(
            float width,
            float height,
            float minDepth,
            float maxDepth)
        {
            VkViewport viewport = new VkViewport();
            viewport.width = width;
            viewport.height = height;
            viewport.minDepth = minDepth;
            viewport.maxDepth = maxDepth;
            return viewport;
        }

        public static VkRect2D rect2D(
            uint width,
            uint height,
            int offsetX,
            int offsetY)
        {
            VkRect2D rect2D = new VkRect2D();
            rect2D.extent.width = width;
            rect2D.extent.height = height;
            rect2D.offset.x = offsetX;
            rect2D.offset.y = offsetY;
            return rect2D;
        }

        public static VkPipelineVertexInputStateCreateInfo pipelineVertexInputStateCreateInfo()
        {
            VkPipelineVertexInputStateCreateInfo pipelineVertexInputStateCreateInfo = VkPipelineVertexInputStateCreateInfo.New();
            return pipelineVertexInputStateCreateInfo;
        }

        public static VkDescriptorPoolCreateInfo descriptorPoolCreateInfo(
            uint poolSizeCount,
            VkDescriptorPoolSize* pPoolSizes,
            uint maxSets)
        {
            VkDescriptorPoolCreateInfo descriptorPoolInfo = VkDescriptorPoolCreateInfo.New();
            descriptorPoolInfo.poolSizeCount = poolSizeCount;
            descriptorPoolInfo.pPoolSizes = pPoolSizes;
            descriptorPoolInfo.maxSets = maxSets;
            return descriptorPoolInfo;
        }

        public static VkDescriptorPoolSize descriptorPoolSize(
            VkDescriptorType type,
            uint descriptorCount)
        {
            VkDescriptorPoolSize descriptorPoolSize = new VkDescriptorPoolSize();
            descriptorPoolSize.type = type;
            descriptorPoolSize.descriptorCount = descriptorCount;
            return descriptorPoolSize;
        }

        public static VkDescriptorSetLayoutBinding descriptorSetLayoutBinding(
            VkDescriptorType type,
            VkShaderStageFlags stageFlags,
            uint binding,
            uint descriptorCount = 1)
        {
            VkDescriptorSetLayoutBinding setLayoutBinding = new VkDescriptorSetLayoutBinding();
            setLayoutBinding.descriptorType = type;
            setLayoutBinding.stageFlags = stageFlags;
            setLayoutBinding.binding = binding;
            setLayoutBinding.descriptorCount = descriptorCount;
            return setLayoutBinding;
        }

        public static VkFramebufferCreateInfo framebufferCreateInfo()
        {
            VkFramebufferCreateInfo framebufferCreateInfo = VkFramebufferCreateInfo.New();
            return framebufferCreateInfo;
        }

        public static VkDescriptorSetLayoutCreateInfo descriptorSetLayoutCreateInfo(
            VkDescriptorSetLayoutBinding* pBindings,
            uint bindingCount)
        {
            VkDescriptorSetLayoutCreateInfo descriptorSetLayoutCreateInfo = VkDescriptorSetLayoutCreateInfo.New();
            descriptorSetLayoutCreateInfo.pBindings = pBindings;
            descriptorSetLayoutCreateInfo.bindingCount = bindingCount;
            return descriptorSetLayoutCreateInfo;
        }

        public static VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo(
            VkDescriptorSetLayout* pSetLayouts,
            uint setLayoutCount = 1)
        {
            VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo = VkPipelineLayoutCreateInfo.New();
            pipelineLayoutCreateInfo.setLayoutCount = setLayoutCount;
            pipelineLayoutCreateInfo.pSetLayouts = pSetLayouts;
            return pipelineLayoutCreateInfo;
        }

        public static VkMappedMemoryRange mappedMemoryRange()
        {
            VkMappedMemoryRange mappedMemoryRange = VkMappedMemoryRange.New();
            return mappedMemoryRange;
        }

        public static VkDescriptorSetAllocateInfo descriptorSetAllocateInfo(
            VkDescriptorPool descriptorPool,
            VkDescriptorSetLayout* pSetLayouts,
            uint descriptorSetCount)
        {
            VkDescriptorSetAllocateInfo descriptorSetAllocateInfo = VkDescriptorSetAllocateInfo.New();
            descriptorSetAllocateInfo.descriptorPool = descriptorPool;
            descriptorSetAllocateInfo.pSetLayouts = pSetLayouts;
            descriptorSetAllocateInfo.descriptorSetCount = descriptorSetCount;
            return descriptorSetAllocateInfo;
        }

        public static VkDescriptorImageInfo descriptorImageInfo(VkSampler sampler, VkImageView imageView, VkImageLayout imageLayout)
        {
            VkDescriptorImageInfo descriptorImageInfo = new VkDescriptorImageInfo();
            descriptorImageInfo.sampler = sampler;
            descriptorImageInfo.imageView = imageView;
            descriptorImageInfo.imageLayout = imageLayout;
            return descriptorImageInfo;
        }

        public static VkPushConstantRange pushConstantRange(
            VkShaderStageFlags stageFlags,
            uint size,
            uint offset)
        {
            VkPushConstantRange pushConstantRange = new VkPushConstantRange();
            pushConstantRange.stageFlags = stageFlags;
            pushConstantRange.offset = offset;
            pushConstantRange.size = size;
            return pushConstantRange;
        }
    }
}
