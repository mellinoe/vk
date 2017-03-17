using System;
using System.Collections.Generic;
using System.IO;
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
                if ((formatProps.optimalTilingFeatures & VkFormatFeatureFlags.DepthStencilAttachment) != 0)
                {
                    *depthFormat = format;
                    return True;
                }
            }

            return False;
        }

        public static VkShaderModule loadShader(string fileName, VkDevice device, VkShaderStageFlags stage)
        {
            using (var fs = File.OpenRead(fileName))
            {
                var length = fs.Length;
            }
            byte[] shaderCode = File.ReadAllBytes(fileName);
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
    }
}
