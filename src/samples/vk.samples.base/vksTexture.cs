// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: base/VulkanTexture.hpp, 

/*
* Vulkan Example base class
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public class vksTexture
    {
        public VkImageView view;
        public VkImage image;
        public VkSampler sampler;
        public VkDeviceMemory deviceMemory;
        public uint width;
        public uint height;
        public uint mipLevels;
        public VkImageLayout imageLayout;
        public VkDescriptorImageInfo descriptor;
    }

    public unsafe class vksTexture2D : vksTexture
    {
        private vksVulkanDevice device;

        /**
        * Load a 2D texture including all mip levels
        *
        * @param filename File to load (supports .ktx and .dds)
        * @param format Vulkan format of the image data stored in the file
        * @param device Vulkan device to create the texture on
        * @param copyQueue Queue used for the texture staging copy commands (must support transfer)
        * @param (Optional) imageUsageFlags Usage flags for the texture's image (defaults to VK_IMAGE_USAGE_SAMPLED_BIT)
        * @param (Optional) imageLayout Usage layout for the texture (defaults VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL)
        * @param (Optional) forceLinear Force linear tiling (not advised, defaults to false)
        *
        */
        public void loadFromFile(
            string filename,
            VkFormat format,
            vksVulkanDevice device,
            VkQueue copyQueue,
            VkImageUsageFlags imageUsageFlags = VkImageUsageFlags.Sampled,
            VkImageLayout imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
            bool forceLinear = false)
        {
            KtxFile tex2D;
            using (var fs = File.OpenRead(filename))
            {
                tex2D = KtxFile.Load(fs, false);
            }

            this.device = device;
            width = tex2D.Header.PixelWidth;
            height = tex2D.Header.PixelHeight;
            if (height == 0) height = width;
            mipLevels = tex2D.Header.NumberOfMipmapLevels;

            // Get device properites for the requested texture format
            VkFormatProperties formatProperties;
            vkGetPhysicalDeviceFormatProperties(device.PhysicalDevice, format, out formatProperties);

            // Only use linear tiling if requested (and supported by the device)
            // Support for linear tiling is mostly limited, so prefer to use
            // optimal tiling instead
            // On most implementations linear tiling will only support a very
            // limited amount of formats and features (mip maps, cubemaps, arrays, etc.)
            bool useStaging = !forceLinear;

            VkMemoryAllocateInfo memAllocInfo = Initializers.memoryAllocateInfo();
            VkMemoryRequirements memReqs;

            // Use a separate command buffer for texture loading
            VkCommandBuffer copyCmd = device.createCommandBuffer(VkCommandBufferLevel.Primary, true);

            if (useStaging)
            {
                // Create a host-visible staging buffer that contains the raw image data
                VkBuffer stagingBuffer;
                VkDeviceMemory stagingMemory;

                VkBufferCreateInfo bufferCreateInfo = Initializers.bufferCreateInfo();
                bufferCreateInfo.size = tex2D.GetTotalSize();
                // This buffer is used as a transfer source for the buffer copy
                bufferCreateInfo.usage = VkBufferUsageFlags.TransferSrc;
                bufferCreateInfo.sharingMode = VkSharingMode.Exclusive;

                Util.CheckResult(vkCreateBuffer(device.LogicalDevice, &bufferCreateInfo, null, &stagingBuffer));

                // Get memory requirements for the staging buffer (alignment, memory type bits)
                vkGetBufferMemoryRequirements(device.LogicalDevice, stagingBuffer, &memReqs);

                memAllocInfo.allocationSize = memReqs.size;
                // Get memory type index for a host visible buffer
                memAllocInfo.memoryTypeIndex = device.getMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

                Util.CheckResult(vkAllocateMemory(device.LogicalDevice, &memAllocInfo, null, &stagingMemory));
                Util.CheckResult(vkBindBufferMemory(device.LogicalDevice, stagingBuffer, stagingMemory, 0));

                // Copy texture data into staging buffer
                byte* data;
                Util.CheckResult(vkMapMemory(device.LogicalDevice, stagingMemory, 0, memReqs.size, 0, (void**)&data));
                byte[] pixelData = tex2D.GetAllTextureData();
                fixed (byte* pixelDataPtr = &pixelData[0])
                {
                    Unsafe.CopyBlock(data, pixelDataPtr, (uint)pixelData.Length);
                }
                vkUnmapMemory(device.LogicalDevice, stagingMemory);

                // Setup buffer copy regions for each mip level
                NativeList<VkBufferImageCopy> bufferCopyRegions = new NativeList<VkBufferImageCopy>();
                uint offset = 0;

                for (uint i = 0; i < mipLevels; i++)
                {
                    VkBufferImageCopy bufferCopyRegion = new VkBufferImageCopy();
                    bufferCopyRegion.imageSubresource.aspectMask = VkImageAspectFlags.Color;
                    bufferCopyRegion.imageSubresource.mipLevel = i;
                    bufferCopyRegion.imageSubresource.baseArrayLayer = 0;
                    bufferCopyRegion.imageSubresource.layerCount = 1;
                    bufferCopyRegion.imageExtent.width = tex2D.Faces[0].Mipmaps[i].Width;
                    bufferCopyRegion.imageExtent.height = tex2D.Faces[0].Mipmaps[i].Height;
                    bufferCopyRegion.imageExtent.depth = 1;
                    bufferCopyRegion.bufferOffset = offset;

                    bufferCopyRegions.Add(bufferCopyRegion);

                    offset += tex2D.Faces[0].Mipmaps[i].SizeInBytes;
                }

                // Create optimal tiled target image
                VkImageCreateInfo imageCreateInfo = Initializers.imageCreateInfo();
                imageCreateInfo.imageType = VkImageType.Image2D;
                imageCreateInfo.format = format;
                imageCreateInfo.mipLevels = mipLevels;
                imageCreateInfo.arrayLayers = 1;
                imageCreateInfo.samples = VkSampleCountFlags.Count1;
                imageCreateInfo.tiling = VkImageTiling.Optimal;
                imageCreateInfo.sharingMode = VkSharingMode.Exclusive;
                imageCreateInfo.initialLayout = VkImageLayout.Undefined;
                imageCreateInfo.extent = new VkExtent3D { width = width, height = height, depth = 1 };
                imageCreateInfo.usage = imageUsageFlags;
                // Ensure that the TRANSFER_DST bit is set for staging
                if ((imageCreateInfo.usage & VkImageUsageFlags.TransferDst) == 0)
                {
                    imageCreateInfo.usage |= VkImageUsageFlags.TransferDst;
                }
                Util.CheckResult(vkCreateImage(device.LogicalDevice, &imageCreateInfo, null, out image));

                vkGetImageMemoryRequirements(device.LogicalDevice, image, &memReqs);

                memAllocInfo.allocationSize = memReqs.size;

                memAllocInfo.memoryTypeIndex = device.getMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);
                Util.CheckResult(vkAllocateMemory(device.LogicalDevice, &memAllocInfo, null, out deviceMemory));
                Util.CheckResult(vkBindImageMemory(device.LogicalDevice, image, deviceMemory, 0));

                VkImageSubresourceRange subresourceRange = new VkImageSubresourceRange();
                subresourceRange.aspectMask = VkImageAspectFlags.Color;
                subresourceRange.baseMipLevel = 0;
                subresourceRange.levelCount = mipLevels;
                subresourceRange.layerCount = 1;

                // Image barrier for optimal image (target)
                // Optimal image will be used as destination for the copy
                Tools.setImageLayout(
                    copyCmd,
                    image,
                    VkImageAspectFlags.Color,
                    VkImageLayout.Undefined,
                    VkImageLayout.TransferDstOptimal,
                    subresourceRange);

                // Copy mip levels from staging buffer
                vkCmdCopyBufferToImage(
                    copyCmd,
                    stagingBuffer,
                    image,
                    VkImageLayout.TransferDstOptimal,
                    bufferCopyRegions.Count,
                    bufferCopyRegions.Data);

                // Change texture image layout to shader read after all mip levels have been copied
                this.imageLayout = imageLayout;
                Tools.setImageLayout(
                    copyCmd,
                    image,
                    VkImageAspectFlags.Color,
                    VkImageLayout.TransferDstOptimal,
                    imageLayout,
                    subresourceRange);

                device.flushCommandBuffer(copyCmd, copyQueue);

                // Clean up staging resources
                vkFreeMemory(device.LogicalDevice, stagingMemory, null);
                vkDestroyBuffer(device.LogicalDevice, stagingBuffer, null);
            }
            else
            {
                throw new NotImplementedException();
                /*
                // Prefer using optimal tiling, as linear tiling 
                // may support only a small set of features 
                // depending on implementation (e.g. no mip maps, only one layer, etc.)

                // Check if this support is supported for linear tiling
                Debug.Assert((formatProperties.linearTilingFeatures & VkFormatFeatureFlags.SampledImage) != 0);

                VkImage mappableImage;
                VkDeviceMemory mappableMemory;

                VkImageCreateInfo imageCreateInfo = Initializers.imageCreateInfo();
                imageCreateInfo.imageType = VkImageType._2d;
                imageCreateInfo.format = format;
                imageCreateInfo.extent = new VkExtent3D { width = width, height = height, depth = 1 };
                imageCreateInfo.mipLevels = 1;
                imageCreateInfo.arrayLayers = 1;
                imageCreateInfo.samples = VkSampleCountFlags._1;
                imageCreateInfo.tiling = VkImageTiling.Linear;
                imageCreateInfo.usage = imageUsageFlags;
                imageCreateInfo.sharingMode = VkSharingMode.Exclusive;
                imageCreateInfo.initialLayout = VkImageLayout.Undefined;

                // Load mip map level 0 to linear tiling image
                Util.CheckResult(vkCreateImage(device.LogicalDevice, &imageCreateInfo, null, &mappableImage));

                // Get memory requirements for this image 
                // like size and alignment
                vkGetImageMemoryRequirements(device.LogicalDevice, mappableImage, &memReqs);
                // Set memory allocation size to required memory size
                memAllocInfo.allocationSize = memReqs.size;

                // Get memory type that can be mapped to host memory
                memAllocInfo.memoryTypeIndex = device.GetMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent);

                // Allocate host memory
                Util.CheckResult(vkAllocateMemory(device.LogicalDevice, &memAllocInfo, null, &mappableMemory));

                // Bind allocated image for use
                Util.CheckResult(vkBindImageMemory(device.LogicalDevice, mappableImage, mappableMemory, 0));

                // Get sub resource layout
                // Mip map count, array layer, etc.
                VkImageSubresource subRes = new VkImageSubresource();
                subRes.aspectMask = VkImageAspectFlags.Color;
                subRes.mipLevel = 0;

                VkSubresourceLayout subResLayout;
                void* data;

                // Get sub resources layout 
                // Includes row pitch, size offsets, etc.
                vkGetImageSubresourceLayout(device.LogicalDevice, mappableImage, &subRes, &subResLayout);

                // Map image memory
                Util.CheckResult(vkMapMemory(device.LogicalDevice, mappableMemory, 0, memReqs.size, 0, &data));

                // Copy image data into memory
                memcpy(data, tex2D[subRes.mipLevel].data(), tex2D[subRes.mipLevel].size());

                vkUnmapMemory(device.LogicalDevice, mappableMemory);

                // Linear tiled images don't need to be staged
                // and can be directly used as textures
                image = mappableImage;
                deviceMemory = mappableMemory;
                imageLayout = imageLayout;

                // Setup image memory barrier
                vks::tools::setImageLayout(copyCmd, image, VkImageAspectFlags.Color, VkImageLayout.Undefined, imageLayout);

                device.flushCommandBuffer(copyCmd, copyQueue);
                */
            }

            // Create a defaultsampler
            VkSamplerCreateInfo samplerCreateInfo = VkSamplerCreateInfo.New();
            samplerCreateInfo.magFilter = VkFilter.Linear;
            samplerCreateInfo.minFilter = VkFilter.Linear;
            samplerCreateInfo.mipmapMode = VkSamplerMipmapMode.Linear;
            samplerCreateInfo.addressModeU = VkSamplerAddressMode.Repeat;
            samplerCreateInfo.addressModeV = VkSamplerAddressMode.Repeat;
            samplerCreateInfo.addressModeW = VkSamplerAddressMode.Repeat;
            samplerCreateInfo.mipLodBias = 0.0f;
            samplerCreateInfo.compareOp = VkCompareOp.Never;
            samplerCreateInfo.minLod = 0.0f;
            // Max level-of-detail should match mip level count
            samplerCreateInfo.maxLod = (useStaging) ? (float)mipLevels : 0.0f;
            // Enable anisotropic filtering
            samplerCreateInfo.maxAnisotropy = 8;
            samplerCreateInfo.anisotropyEnable = True;
            samplerCreateInfo.borderColor = VkBorderColor.FloatOpaqueWhite;
            Util.CheckResult(vkCreateSampler(device.LogicalDevice, &samplerCreateInfo, null, out sampler));

            // Create image view
            // Textures are not directly accessed by the shaders and
            // are abstracted by image views containing additional
            // information and sub resource ranges
            VkImageViewCreateInfo viewCreateInfo = VkImageViewCreateInfo.New();
            viewCreateInfo.viewType = VkImageViewType.Image2D;
            viewCreateInfo.format = format;
            viewCreateInfo.components = new VkComponentMapping { r = VkComponentSwizzle.R, g = VkComponentSwizzle.G, b = VkComponentSwizzle.B, a = VkComponentSwizzle.A };
            viewCreateInfo.subresourceRange = new VkImageSubresourceRange { aspectMask = VkImageAspectFlags.Color, baseMipLevel = 0, levelCount = 1, baseArrayLayer = 0, layerCount = 1 };
            // Linear tiling usually won't support mip maps
            // Only set mip map count if optimal tiling is used
            viewCreateInfo.subresourceRange.levelCount = (useStaging) ? mipLevels : 1;
            viewCreateInfo.image = image;
            Util.CheckResult(vkCreateImageView(device.LogicalDevice, &viewCreateInfo, null, out view));

            // Update descriptor image info member that can be used for setting up descriptor sets
            updateDescriptor();
        }

        /** @brief Update image descriptor from current sampler, view and image layout */
        void updateDescriptor()
        {
            descriptor.sampler = sampler;
            descriptor.imageView = view;
            descriptor.imageLayout = imageLayout;
        }

        public void destroy() { }
    }
}
