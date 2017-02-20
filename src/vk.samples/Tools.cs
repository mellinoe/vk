using System.Collections.Generic;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class Tools
    {
        public static uint getSupportedDepthFormat(VkPhysicalDevice physicalDevice, VkFormat* depthFormat)
        {
            // Since all depth formats may be optional, we need to find a suitable depth format to use
            // Start with the highest precision packed format
            List<VkFormat> depthFormats = new List<VkFormat>()
            {
                VkFormat.D32SfloatS8Uint,
                VkFormat.D32Sfloat,
                VkFormat.D24UnormS8Uint,
                VkFormat.D16UnormS8Uint,
                VkFormat.D16Unorm,
            };

            foreach (VkFormat format in depthFormats)
            {
                VkFormatProperties formatProps;
                vkGetPhysicalDeviceFormatProperties(physicalDevice, format, &formatProps);
                // Format must support depth stencil attachment for optimal tiling
                if ((formatProps.optimalTilingFeatures & (uint)VkFormatFeatureFlagBits.DepthStencilAttachment) != 0)
                {
                    *depthFormat = format;
                    return True;
                }
            }

            return False;
        }
    }
}
