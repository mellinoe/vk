using System;
using Vulkan;

namespace Vk.Samples
{
    public static class Initializers
    {
        public static VkSemaphoreCreateInfo SemaphoreCreateInfo()
        {
            VkSemaphoreCreateInfo semaphoreCreateInfo = new VkSemaphoreCreateInfo();
            semaphoreCreateInfo.sType =  VkStructureType.SemaphoreCreateInfo;
            return semaphoreCreateInfo;
        }


        public static VkSubmitInfo SubmitInfo()
        {
            VkSubmitInfo submitInfo = new VkSubmitInfo();
            submitInfo.sType =  VkStructureType.SubmitInfo;
            return submitInfo;
        }

        public static VkCommandBufferAllocateInfo CommandBufferAllocateInfo(
            VkCommandPool commandPool,
            VkCommandBufferLevel level,
            uint bufferCount)
        {
            VkCommandBufferAllocateInfo commandBufferAllocateInfo = new VkCommandBufferAllocateInfo();
            commandBufferAllocateInfo.sType =  VkStructureType.CommandBufferAllocateInfo;
            commandBufferAllocateInfo.commandPool = commandPool;
            commandBufferAllocateInfo.level = level;
            commandBufferAllocateInfo.commandBufferCount = bufferCount;
            return commandBufferAllocateInfo;
        }

        public static VkCommandBufferBeginInfo CommandBufferBeginInfo()
        {
            VkCommandBufferBeginInfo cmdBufferBeginInfo = new VkCommandBufferBeginInfo();
            cmdBufferBeginInfo.sType =  VkStructureType.CommandBufferBeginInfo;
            return cmdBufferBeginInfo;
        }
    }
}
