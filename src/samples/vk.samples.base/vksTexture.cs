using Vulkan;

namespace Vk.Samples
{
    public class vksTexture
    {
        public VkImageView view;
        public VkImage image;
        public VkSampler sampler;
        public VkDeviceMemory DeviceMemory;
        public uint width;
        public uint height;
        public uint mipLevels;
        public VkImageLayout imageLayout;
    }
}
