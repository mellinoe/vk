using System;
using Vulkan;

namespace Vk.Samples
{
    // Temporary workarounds for things that should be in vk.dll
    public static class Hacks
    {
        public static VkStructureType VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR = (VkStructureType)1000009000;
        public static VkStructureType VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR = (VkStructureType)1000001000;
        public static VkStructureType VK_STRUCTURE_TYPE_PRESENT_INFO_KHR = (VkStructureType)1000001001;
        public static VkImageLayout VK_IMAGE_LAYOUT_PRESENT_SRC_KHR = (VkImageLayout)1000001002;
        public static readonly IntPtr VK_NULL_HANDLE = IntPtr.Zero;
    }
}
