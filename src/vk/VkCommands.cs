using System;
using System.Collections.Generic;
using System.Text;

namespace Vulkan
{
    public static unsafe partial class VulkanNative
    {
        public static VkResult vkQueueSubmit(VkQueue queue, params VkSubmitInfo[] submits)
        {
            fixed (VkSubmitInfo* pSubmits = &submits[0])
            {
                uint count = (uint)submits.Length;
                return vkQueueSubmit(queue, count, pSubmits, VkFence.Null);
            }
        }

        public static VkResult vkQueueSubmit(VkQueue queue, VkSubmitInfo[] submits, VkFence fence)
        {
            fixed (VkSubmitInfo* pSubmits = &submits[0])
            {
                uint count = (uint)submits.Length;
                return vkQueueSubmit(queue, count, pSubmits, fence);
            }
        }

        public static VkResult vkQueueSubmit(VkQueue queue, VkSubmitInfo submit, VkFence fence)
        {
            return vkQueueSubmit(queue, 1, &submit, fence);
        }

        public static VkResult vkQueueSubmit(
            VkQueue queue,
            VkSemaphore waitSemaphore,
            VkPipelineStageFlags waitDstStageMask,
            VkCommandBuffer commandBuffer,
            VkSemaphore signalSemaphore,
            VkFence fence)
        {
            var submitInfo = new VkSubmitInfo
            {
                sType = VkStructureType.SubmitInfo,
                commandBufferCount = 1,
                pCommandBuffers = &commandBuffer,
                waitSemaphoreCount = 1,
                pWaitSemaphores = &waitSemaphore,
                pWaitDstStageMask = &waitDstStageMask,
                signalSemaphoreCount = 1,
                pSignalSemaphores = &signalSemaphore
            };

            return vkQueueSubmit(queue, 1, &submitInfo, fence);
        }

        public static VkResult vkQueueSubmit(
            VkQueue queue,
            VkSemaphore waitSemaphore,
            VkPipelineStageFlags waitDstStageMask,
            VkCommandBuffer commandBuffer,
            VkSemaphore signalSemaphore)
        {
            var submitInfo = new VkSubmitInfo
            {
                sType = VkStructureType.SubmitInfo,
                commandBufferCount = 1,
                pCommandBuffers = &commandBuffer,
                waitSemaphoreCount = 1,
                pWaitSemaphores = &waitSemaphore,
                pWaitDstStageMask = &waitDstStageMask,
                signalSemaphoreCount = 1,
                pSignalSemaphores = &signalSemaphore
            };

            return vkQueueSubmit(queue, 1, &submitInfo, VkFence.Null);
        }

        public static VkResult vkQueueSubmit(
            VkQueue queue,
            VkSemaphore[] waitSemaphores,
            VkPipelineStageFlags[] waitDstStageMasks,
            VkCommandBuffer commandBuffer,
            VkSemaphore[] signalSemaphores)
        {
            fixed (VkSemaphore* pWaitSemaphores = &waitSemaphores[0])
            {
                fixed (VkPipelineStageFlags* pWaitDstStageMask = &waitDstStageMasks[0])
                {
                    fixed (VkSemaphore* pSignalSemaphores = &signalSemaphores[0])
                    {
                        var submitInfo = new VkSubmitInfo
                        {
                            sType = VkStructureType.SubmitInfo,
                            commandBufferCount = 1,
                            pCommandBuffers = &commandBuffer,
                            waitSemaphoreCount = (uint)waitSemaphores.Length,
                            pWaitSemaphores = pWaitSemaphores,
                            pWaitDstStageMask = pWaitDstStageMask,
                            signalSemaphoreCount = (uint)signalSemaphores.Length,
                            pSignalSemaphores = pSignalSemaphores
                        };

                        return vkQueueSubmit(queue, 1, &submitInfo, VkFence.Null);
                    }
                }
            }
        }
    }
}
