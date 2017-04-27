// This file is generated.

#if CALLI_STUBS
using System;

namespace Vulkan
{
    public unsafe partial struct VkOffset2D
    {
        public int x;
        public int y;
    }

    public unsafe partial struct VkOffset3D
    {
        public int x;
        public int y;
        public int z;
    }

    public unsafe partial struct VkExtent2D
    {
        public uint width;
        public uint height;
    }

    public unsafe partial struct VkExtent3D
    {
        public uint width;
        public uint height;
        public uint depth;
    }

    public unsafe partial struct VkViewport
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public float minDepth;
        public float maxDepth;
    }

    public unsafe partial struct VkRect2D
    {
        public VkOffset2D offset;
        public VkExtent2D extent;
    }

    public unsafe partial struct VkRect3D
    {
        public VkOffset3D offset;
        public VkExtent3D extent;
    }

    public unsafe partial struct VkClearRect
    {
        public VkRect2D rect;
        public uint baseArrayLayer;
        public uint layerCount;
    }

    public unsafe partial struct VkComponentMapping
    {
        public VkComponentSwizzle r;
        public VkComponentSwizzle g;
        public VkComponentSwizzle b;
        public VkComponentSwizzle a;
    }

    public unsafe partial struct VkPhysicalDeviceProperties
    {
        public uint apiVersion;
        public uint driverVersion;
        public uint vendorID;
        public uint deviceID;
        public VkPhysicalDeviceType deviceType;
        public fixed byte deviceName[(int)VulkanNative.MaxPhysicalDeviceNameSize];
        public fixed byte pipelineCacheUUID[(int)VulkanNative.UuidSize];
        public VkPhysicalDeviceLimits limits;
        public VkPhysicalDeviceSparseProperties sparseProperties;
    }

    public unsafe partial struct VkExtensionProperties
    {
        public fixed byte extensionName[(int)VulkanNative.MaxExtensionNameSize];
        public uint specVersion;
    }

    public unsafe partial struct VkLayerProperties
    {
        public fixed byte layerName[(int)VulkanNative.MaxExtensionNameSize];
        public uint specVersion;
        public uint implementationVersion;
        public fixed byte description[(int)VulkanNative.MaxDescriptionSize];
    }

    public unsafe partial struct VkApplicationInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public byte* pApplicationName;
        public uint applicationVersion;
        public byte* pEngineName;
        public uint engineVersion;
        public uint apiVersion;
        public static VkApplicationInfo New()
        {
            VkApplicationInfo ret = new VkApplicationInfo();
            ret.sType = VkStructureType.ApplicationInfo;
            return ret;
        }
    }

    public unsafe partial struct VkAllocationCallbacks
    {
        public void* pUserData;
        public IntPtr pfnAllocation;
        public IntPtr pfnReallocation;
        public IntPtr pfnFree;
        public IntPtr pfnInternalAllocation;
        public IntPtr pfnInternalFree;
    }

    public unsafe partial struct VkDeviceQueueCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint queueFamilyIndex;
        public uint queueCount;
        public float* pQueuePriorities;
        public static VkDeviceQueueCreateInfo New()
        {
            VkDeviceQueueCreateInfo ret = new VkDeviceQueueCreateInfo();
            ret.sType = VkStructureType.DeviceQueueCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint queueCreateInfoCount;
        public VkDeviceQueueCreateInfo* pQueueCreateInfos;
        public uint enabledLayerCount;
        public byte** ppEnabledLayerNames;
        public uint enabledExtensionCount;
        public byte** ppEnabledExtensionNames;
        public VkPhysicalDeviceFeatures* pEnabledFeatures;
        public static VkDeviceCreateInfo New()
        {
            VkDeviceCreateInfo ret = new VkDeviceCreateInfo();
            ret.sType = VkStructureType.DeviceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkInstanceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkApplicationInfo* pApplicationInfo;
        public uint enabledLayerCount;
        public byte** ppEnabledLayerNames;
        public uint enabledExtensionCount;
        public byte** ppEnabledExtensionNames;
        public static VkInstanceCreateInfo New()
        {
            VkInstanceCreateInfo ret = new VkInstanceCreateInfo();
            ret.sType = VkStructureType.InstanceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkQueueFamilyProperties
    {
        public VkQueueFlags queueFlags;
        public uint queueCount;
        public uint timestampValidBits;
        public VkExtent3D minImageTransferGranularity;
    }

    public unsafe partial struct VkPhysicalDeviceMemoryProperties
    {
        public uint memoryTypeCount;
        public VkMemoryType memoryTypes_0;
        public VkMemoryType memoryTypes_1;
        public VkMemoryType memoryTypes_2;
        public VkMemoryType memoryTypes_3;
        public VkMemoryType memoryTypes_4;
        public VkMemoryType memoryTypes_5;
        public VkMemoryType memoryTypes_6;
        public VkMemoryType memoryTypes_7;
        public VkMemoryType memoryTypes_8;
        public VkMemoryType memoryTypes_9;
        public VkMemoryType memoryTypes_10;
        public VkMemoryType memoryTypes_11;
        public VkMemoryType memoryTypes_12;
        public VkMemoryType memoryTypes_13;
        public VkMemoryType memoryTypes_14;
        public VkMemoryType memoryTypes_15;
        public VkMemoryType memoryTypes_16;
        public VkMemoryType memoryTypes_17;
        public VkMemoryType memoryTypes_18;
        public VkMemoryType memoryTypes_19;
        public VkMemoryType memoryTypes_20;
        public VkMemoryType memoryTypes_21;
        public VkMemoryType memoryTypes_22;
        public VkMemoryType memoryTypes_23;
        public VkMemoryType memoryTypes_24;
        public VkMemoryType memoryTypes_25;
        public VkMemoryType memoryTypes_26;
        public VkMemoryType memoryTypes_27;
        public VkMemoryType memoryTypes_28;
        public VkMemoryType memoryTypes_29;
        public VkMemoryType memoryTypes_30;
        public VkMemoryType memoryTypes_31;
        public uint memoryHeapCount;
        public VkMemoryHeap memoryHeaps_0;
        public VkMemoryHeap memoryHeaps_1;
        public VkMemoryHeap memoryHeaps_2;
        public VkMemoryHeap memoryHeaps_3;
        public VkMemoryHeap memoryHeaps_4;
        public VkMemoryHeap memoryHeaps_5;
        public VkMemoryHeap memoryHeaps_6;
        public VkMemoryHeap memoryHeaps_7;
        public VkMemoryHeap memoryHeaps_8;
        public VkMemoryHeap memoryHeaps_9;
        public VkMemoryHeap memoryHeaps_10;
        public VkMemoryHeap memoryHeaps_11;
        public VkMemoryHeap memoryHeaps_12;
        public VkMemoryHeap memoryHeaps_13;
        public VkMemoryHeap memoryHeaps_14;
        public VkMemoryHeap memoryHeaps_15;
    }

    public unsafe partial struct VkMemoryAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public ulong allocationSize;
        public uint memoryTypeIndex;
        public static VkMemoryAllocateInfo New()
        {
            VkMemoryAllocateInfo ret = new VkMemoryAllocateInfo();
            ret.sType = VkStructureType.MemoryAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkMemoryRequirements
    {
        public ulong size;
        public ulong alignment;
        public uint memoryTypeBits;
    }

    public unsafe partial struct VkSparseImageFormatProperties
    {
        public VkImageAspectFlags aspectMask;
        public VkExtent3D imageGranularity;
        public VkSparseImageFormatFlags flags;
    }

    public unsafe partial struct VkSparseImageMemoryRequirements
    {
        public VkSparseImageFormatProperties formatProperties;
        public uint imageMipTailFirstLod;
        public ulong imageMipTailSize;
        public ulong imageMipTailOffset;
        public ulong imageMipTailStride;
    }

    public unsafe partial struct VkMemoryType
    {
        public VkMemoryPropertyFlags propertyFlags;
        public uint heapIndex;
    }

    public unsafe partial struct VkMemoryHeap
    {
        public ulong size;
        public VkMemoryHeapFlags flags;
    }

    public unsafe partial struct VkMappedMemoryRange
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDeviceMemory memory;
        public ulong offset;
        public ulong size;
        public static VkMappedMemoryRange New()
        {
            VkMappedMemoryRange ret = new VkMappedMemoryRange();
            ret.sType = VkStructureType.MappedMemoryRange;
            return ret;
        }
    }

    public unsafe partial struct VkFormatProperties
    {
        public VkFormatFeatureFlags linearTilingFeatures;
        public VkFormatFeatureFlags optimalTilingFeatures;
        public VkFormatFeatureFlags bufferFeatures;
    }

    public unsafe partial struct VkImageFormatProperties
    {
        public VkExtent3D maxExtent;
        public uint maxMipLevels;
        public uint maxArrayLayers;
        public VkSampleCountFlags sampleCounts;
        public ulong maxResourceSize;
    }

    public unsafe partial struct VkDescriptorBufferInfo
    {
        public VkBuffer buffer;
        public ulong offset;
        public ulong range;
    }

    public unsafe partial struct VkDescriptorImageInfo
    {
        public VkSampler sampler;
        public VkImageView imageView;
        public VkImageLayout imageLayout;
    }

    public unsafe partial struct VkWriteDescriptorSet
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSet dstSet;
        public uint dstBinding;
        public uint dstArrayElement;
        public uint descriptorCount;
        public VkDescriptorType descriptorType;
        public VkDescriptorImageInfo* pImageInfo;
        public VkDescriptorBufferInfo* pBufferInfo;
        public VkBufferView* pTexelBufferView;
        public static VkWriteDescriptorSet New()
        {
            VkWriteDescriptorSet ret = new VkWriteDescriptorSet();
            ret.sType = VkStructureType.WriteDescriptorSet;
            return ret;
        }
    }

    public unsafe partial struct VkCopyDescriptorSet
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorSet srcSet;
        public uint srcBinding;
        public uint srcArrayElement;
        public VkDescriptorSet dstSet;
        public uint dstBinding;
        public uint dstArrayElement;
        public uint descriptorCount;
        public static VkCopyDescriptorSet New()
        {
            VkCopyDescriptorSet ret = new VkCopyDescriptorSet();
            ret.sType = VkStructureType.CopyDescriptorSet;
            return ret;
        }
    }

    public unsafe partial struct VkBufferCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBufferCreateFlags flags;
        public ulong size;
        public VkBufferUsageFlags usage;
        public VkSharingMode sharingMode;
        public uint queueFamilyIndexCount;
        public uint* pQueueFamilyIndices;
        public static VkBufferCreateInfo New()
        {
            VkBufferCreateInfo ret = new VkBufferCreateInfo();
            ret.sType = VkStructureType.BufferCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkBufferViewCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBuffer buffer;
        public VkFormat format;
        public ulong offset;
        public ulong range;
        public static VkBufferViewCreateInfo New()
        {
            VkBufferViewCreateInfo ret = new VkBufferViewCreateInfo();
            ret.sType = VkStructureType.BufferViewCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkImageSubresource
    {
        public VkImageAspectFlags aspectMask;
        public uint mipLevel;
        public uint arrayLayer;
    }

    public unsafe partial struct VkImageSubresourceLayers
    {
        public VkImageAspectFlags aspectMask;
        public uint mipLevel;
        public uint baseArrayLayer;
        public uint layerCount;
    }

    public unsafe partial struct VkImageSubresourceRange
    {
        public VkImageAspectFlags aspectMask;
        public uint baseMipLevel;
        public uint levelCount;
        public uint baseArrayLayer;
        public uint layerCount;
    }

    public unsafe partial struct VkMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public static VkMemoryBarrier New()
        {
            VkMemoryBarrier ret = new VkMemoryBarrier();
            ret.sType = VkStructureType.MemoryBarrier;
            return ret;
        }
    }

    public unsafe partial struct VkBufferMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public uint srcQueueFamilyIndex;
        public uint dstQueueFamilyIndex;
        public VkBuffer buffer;
        public ulong offset;
        public ulong size;
        public static VkBufferMemoryBarrier New()
        {
            VkBufferMemoryBarrier ret = new VkBufferMemoryBarrier();
            ret.sType = VkStructureType.BufferMemoryBarrier;
            return ret;
        }
    }

    public unsafe partial struct VkImageMemoryBarrier
    {
        public VkStructureType sType;
        public void* pNext;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public VkImageLayout oldLayout;
        public VkImageLayout newLayout;
        public uint srcQueueFamilyIndex;
        public uint dstQueueFamilyIndex;
        public VkImage image;
        public VkImageSubresourceRange subresourceRange;
        public static VkImageMemoryBarrier New()
        {
            VkImageMemoryBarrier ret = new VkImageMemoryBarrier();
            ret.sType = VkStructureType.ImageMemoryBarrier;
            return ret;
        }
    }

    public unsafe partial struct VkImageCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImageCreateFlags flags;
        public VkImageType imageType;
        public VkFormat format;
        public VkExtent3D extent;
        public uint mipLevels;
        public uint arrayLayers;
        public VkSampleCountFlags samples;
        public VkImageTiling tiling;
        public VkImageUsageFlags usage;
        public VkSharingMode sharingMode;
        public uint queueFamilyIndexCount;
        public uint* pQueueFamilyIndices;
        public VkImageLayout initialLayout;
        public static VkImageCreateInfo New()
        {
            VkImageCreateInfo ret = new VkImageCreateInfo();
            ret.sType = VkStructureType.ImageCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSubresourceLayout
    {
        public ulong offset;
        public ulong size;
        public ulong rowPitch;
        public ulong arrayPitch;
        public ulong depthPitch;
    }

    public unsafe partial struct VkImageViewCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkImage image;
        public VkImageViewType viewType;
        public VkFormat format;
        public VkComponentMapping components;
        public VkImageSubresourceRange subresourceRange;
        public static VkImageViewCreateInfo New()
        {
            VkImageViewCreateInfo ret = new VkImageViewCreateInfo();
            ret.sType = VkStructureType.ImageViewCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkBufferCopy
    {
        public ulong srcOffset;
        public ulong dstOffset;
        public ulong size;
    }

    public unsafe partial struct VkSparseMemoryBind
    {
        public ulong resourceOffset;
        public ulong size;
        public VkDeviceMemory memory;
        public ulong memoryOffset;
        public VkSparseMemoryBindFlags flags;
    }

    public unsafe partial struct VkSparseImageMemoryBind
    {
        public VkImageSubresource subresource;
        public VkOffset3D offset;
        public VkExtent3D extent;
        public VkDeviceMemory memory;
        public ulong memoryOffset;
        public VkSparseMemoryBindFlags flags;
    }

    public unsafe partial struct VkSparseBufferMemoryBindInfo
    {
        public VkBuffer buffer;
        public uint bindCount;
        public VkSparseMemoryBind* pBinds;
    }

    public unsafe partial struct VkSparseImageOpaqueMemoryBindInfo
    {
        public VkImage image;
        public uint bindCount;
        public VkSparseMemoryBind* pBinds;
    }

    public unsafe partial struct VkSparseImageMemoryBindInfo
    {
        public VkImage image;
        public uint bindCount;
        public VkSparseImageMemoryBind* pBinds;
    }

    public unsafe partial struct VkBindSparseInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public uint bufferBindCount;
        public VkSparseBufferMemoryBindInfo* pBufferBinds;
        public uint imageOpaqueBindCount;
        public VkSparseImageOpaqueMemoryBindInfo* pImageOpaqueBinds;
        public uint imageBindCount;
        public VkSparseImageMemoryBindInfo* pImageBinds;
        public uint signalSemaphoreCount;
        public VkSemaphore* pSignalSemaphores;
        public static VkBindSparseInfo New()
        {
            VkBindSparseInfo ret = new VkBindSparseInfo();
            ret.sType = VkStructureType.BindSparseInfo;
            return ret;
        }
    }

    public unsafe partial struct VkImageCopy
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffset;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffset;
        public VkExtent3D extent;
    }

    public unsafe partial struct VkImageBlit
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffsets_0;
        public VkOffset3D srcOffsets_1;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffsets_0;
        public VkOffset3D dstOffsets_1;
    }

    public unsafe partial struct VkBufferImageCopy
    {
        public ulong bufferOffset;
        public uint bufferRowLength;
        public uint bufferImageHeight;
        public VkImageSubresourceLayers imageSubresource;
        public VkOffset3D imageOffset;
        public VkExtent3D imageExtent;
    }

    public unsafe partial struct VkImageResolve
    {
        public VkImageSubresourceLayers srcSubresource;
        public VkOffset3D srcOffset;
        public VkImageSubresourceLayers dstSubresource;
        public VkOffset3D dstOffset;
        public VkExtent3D extent;
    }

    public unsafe partial struct VkShaderModuleCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public UIntPtr codeSize;
        public uint* pCode;
        public static VkShaderModuleCreateInfo New()
        {
            VkShaderModuleCreateInfo ret = new VkShaderModuleCreateInfo();
            ret.sType = VkStructureType.ShaderModuleCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorSetLayoutBinding
    {
        public uint binding;
        public VkDescriptorType descriptorType;
        public uint descriptorCount;
        public VkShaderStageFlags stageFlags;
        public VkSampler* pImmutableSamplers;
    }

    public unsafe partial struct VkDescriptorSetLayoutCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint bindingCount;
        public VkDescriptorSetLayoutBinding* pBindings;
        public static VkDescriptorSetLayoutCreateInfo New()
        {
            VkDescriptorSetLayoutCreateInfo ret = new VkDescriptorSetLayoutCreateInfo();
            ret.sType = VkStructureType.DescriptorSetLayoutCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorPoolSize
    {
        public VkDescriptorType type;
        public uint descriptorCount;
    }

    public unsafe partial struct VkDescriptorPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorPoolCreateFlags flags;
        public uint maxSets;
        public uint poolSizeCount;
        public VkDescriptorPoolSize* pPoolSizes;
        public static VkDescriptorPoolCreateInfo New()
        {
            VkDescriptorPoolCreateInfo ret = new VkDescriptorPoolCreateInfo();
            ret.sType = VkStructureType.DescriptorPoolCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDescriptorSetAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDescriptorPool descriptorPool;
        public uint descriptorSetCount;
        public VkDescriptorSetLayout* pSetLayouts;
        public static VkDescriptorSetAllocateInfo New()
        {
            VkDescriptorSetAllocateInfo ret = new VkDescriptorSetAllocateInfo();
            ret.sType = VkStructureType.DescriptorSetAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSpecializationMapEntry
    {
        public uint constantID;
        public uint offset;
        public UIntPtr size;
    }

    public unsafe partial struct VkSpecializationInfo
    {
        public uint mapEntryCount;
        public VkSpecializationMapEntry* pMapEntries;
        public UIntPtr dataSize;
        public void* pData;
    }

    public unsafe partial struct VkPipelineShaderStageCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkShaderStageFlags stage;
        public VkShaderModule module;
        public byte* pName;
        public VkSpecializationInfo* pSpecializationInfo;
        public static VkPipelineShaderStageCreateInfo New()
        {
            VkPipelineShaderStageCreateInfo ret = new VkPipelineShaderStageCreateInfo();
            ret.sType = VkStructureType.PipelineShaderStageCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkComputePipelineCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineCreateFlags flags;
        public VkPipelineShaderStageCreateInfo stage;
        public VkPipelineLayout layout;
        public VkPipeline basePipelineHandle;
        public int basePipelineIndex;
        public static VkComputePipelineCreateInfo New()
        {
            VkComputePipelineCreateInfo ret = new VkComputePipelineCreateInfo();
            ret.sType = VkStructureType.ComputePipelineCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkVertexInputBindingDescription
    {
        public uint binding;
        public uint stride;
        public VkVertexInputRate inputRate;
    }

    public unsafe partial struct VkVertexInputAttributeDescription
    {
        public uint location;
        public uint binding;
        public VkFormat format;
        public uint offset;
    }

    public unsafe partial struct VkPipelineVertexInputStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint vertexBindingDescriptionCount;
        public VkVertexInputBindingDescription* pVertexBindingDescriptions;
        public uint vertexAttributeDescriptionCount;
        public VkVertexInputAttributeDescription* pVertexAttributeDescriptions;
        public static VkPipelineVertexInputStateCreateInfo New()
        {
            VkPipelineVertexInputStateCreateInfo ret = new VkPipelineVertexInputStateCreateInfo();
            ret.sType = VkStructureType.PipelineVertexInputStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineInputAssemblyStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkPrimitiveTopology topology;
        public VkBool32 primitiveRestartEnable;
        public static VkPipelineInputAssemblyStateCreateInfo New()
        {
            VkPipelineInputAssemblyStateCreateInfo ret = new VkPipelineInputAssemblyStateCreateInfo();
            ret.sType = VkStructureType.PipelineInputAssemblyStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineTessellationStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint patchControlPoints;
        public static VkPipelineTessellationStateCreateInfo New()
        {
            VkPipelineTessellationStateCreateInfo ret = new VkPipelineTessellationStateCreateInfo();
            ret.sType = VkStructureType.PipelineTessellationStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineViewportStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint viewportCount;
        public VkViewport* pViewports;
        public uint scissorCount;
        public VkRect2D* pScissors;
        public static VkPipelineViewportStateCreateInfo New()
        {
            VkPipelineViewportStateCreateInfo ret = new VkPipelineViewportStateCreateInfo();
            ret.sType = VkStructureType.PipelineViewportStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineRasterizationStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBool32 depthClampEnable;
        public VkBool32 rasterizerDiscardEnable;
        public VkPolygonMode polygonMode;
        public VkCullModeFlags cullMode;
        public VkFrontFace frontFace;
        public VkBool32 depthBiasEnable;
        public float depthBiasConstantFactor;
        public float depthBiasClamp;
        public float depthBiasSlopeFactor;
        public float lineWidth;
        public static VkPipelineRasterizationStateCreateInfo New()
        {
            VkPipelineRasterizationStateCreateInfo ret = new VkPipelineRasterizationStateCreateInfo();
            ret.sType = VkStructureType.PipelineRasterizationStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineMultisampleStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkSampleCountFlags rasterizationSamples;
        public VkBool32 sampleShadingEnable;
        public float minSampleShading;
        public uint* pSampleMask;
        public VkBool32 alphaToCoverageEnable;
        public VkBool32 alphaToOneEnable;
        public static VkPipelineMultisampleStateCreateInfo New()
        {
            VkPipelineMultisampleStateCreateInfo ret = new VkPipelineMultisampleStateCreateInfo();
            ret.sType = VkStructureType.PipelineMultisampleStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineColorBlendAttachmentState
    {
        public VkBool32 blendEnable;
        public VkBlendFactor srcColorBlendFactor;
        public VkBlendFactor dstColorBlendFactor;
        public VkBlendOp colorBlendOp;
        public VkBlendFactor srcAlphaBlendFactor;
        public VkBlendFactor dstAlphaBlendFactor;
        public VkBlendOp alphaBlendOp;
        public VkColorComponentFlags colorWriteMask;
    }

    public unsafe partial struct VkPipelineColorBlendStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBool32 logicOpEnable;
        public VkLogicOp logicOp;
        public uint attachmentCount;
        public VkPipelineColorBlendAttachmentState* pAttachments;
        public float blendConstants_0;
        public float blendConstants_1;
        public float blendConstants_2;
        public float blendConstants_3;
        public static VkPipelineColorBlendStateCreateInfo New()
        {
            VkPipelineColorBlendStateCreateInfo ret = new VkPipelineColorBlendStateCreateInfo();
            ret.sType = VkStructureType.PipelineColorBlendStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineDynamicStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint dynamicStateCount;
        public VkDynamicState* pDynamicStates;
        public static VkPipelineDynamicStateCreateInfo New()
        {
            VkPipelineDynamicStateCreateInfo ret = new VkPipelineDynamicStateCreateInfo();
            ret.sType = VkStructureType.PipelineDynamicStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkStencilOpState
    {
        public VkStencilOp failOp;
        public VkStencilOp passOp;
        public VkStencilOp depthFailOp;
        public VkCompareOp compareOp;
        public uint compareMask;
        public uint writeMask;
        public uint reference;
    }

    public unsafe partial struct VkPipelineDepthStencilStateCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkBool32 depthTestEnable;
        public VkBool32 depthWriteEnable;
        public VkCompareOp depthCompareOp;
        public VkBool32 depthBoundsTestEnable;
        public VkBool32 stencilTestEnable;
        public VkStencilOpState front;
        public VkStencilOpState back;
        public float minDepthBounds;
        public float maxDepthBounds;
        public static VkPipelineDepthStencilStateCreateInfo New()
        {
            VkPipelineDepthStencilStateCreateInfo ret = new VkPipelineDepthStencilStateCreateInfo();
            ret.sType = VkStructureType.PipelineDepthStencilStateCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkGraphicsPipelineCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineCreateFlags flags;
        public uint stageCount;
        public VkPipelineShaderStageCreateInfo* pStages;
        public VkPipelineVertexInputStateCreateInfo* pVertexInputState;
        public VkPipelineInputAssemblyStateCreateInfo* pInputAssemblyState;
        public VkPipelineTessellationStateCreateInfo* pTessellationState;
        public VkPipelineViewportStateCreateInfo* pViewportState;
        public VkPipelineRasterizationStateCreateInfo* pRasterizationState;
        public VkPipelineMultisampleStateCreateInfo* pMultisampleState;
        public VkPipelineDepthStencilStateCreateInfo* pDepthStencilState;
        public VkPipelineColorBlendStateCreateInfo* pColorBlendState;
        public VkPipelineDynamicStateCreateInfo* pDynamicState;
        public VkPipelineLayout layout;
        public VkRenderPass renderPass;
        public uint subpass;
        public VkPipeline basePipelineHandle;
        public int basePipelineIndex;
        public static VkGraphicsPipelineCreateInfo New()
        {
            VkGraphicsPipelineCreateInfo ret = new VkGraphicsPipelineCreateInfo();
            ret.sType = VkStructureType.GraphicsPipelineCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPipelineCacheCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public UIntPtr initialDataSize;
        public void* pInitialData;
        public static VkPipelineCacheCreateInfo New()
        {
            VkPipelineCacheCreateInfo ret = new VkPipelineCacheCreateInfo();
            ret.sType = VkStructureType.PipelineCacheCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPushConstantRange
    {
        public VkShaderStageFlags stageFlags;
        public uint offset;
        public uint size;
    }

    public unsafe partial struct VkPipelineLayoutCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint setLayoutCount;
        public VkDescriptorSetLayout* pSetLayouts;
        public uint pushConstantRangeCount;
        public VkPushConstantRange* pPushConstantRanges;
        public static VkPipelineLayoutCreateInfo New()
        {
            VkPipelineLayoutCreateInfo ret = new VkPipelineLayoutCreateInfo();
            ret.sType = VkStructureType.PipelineLayoutCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSamplerCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkFilter magFilter;
        public VkFilter minFilter;
        public VkSamplerMipmapMode mipmapMode;
        public VkSamplerAddressMode addressModeU;
        public VkSamplerAddressMode addressModeV;
        public VkSamplerAddressMode addressModeW;
        public float mipLodBias;
        public VkBool32 anisotropyEnable;
        public float maxAnisotropy;
        public VkBool32 compareEnable;
        public VkCompareOp compareOp;
        public float minLod;
        public float maxLod;
        public VkBorderColor borderColor;
        public VkBool32 unnormalizedCoordinates;
        public static VkSamplerCreateInfo New()
        {
            VkSamplerCreateInfo ret = new VkSamplerCreateInfo();
            ret.sType = VkStructureType.SamplerCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandPoolCreateFlags flags;
        public uint queueFamilyIndex;
        public static VkCommandPoolCreateInfo New()
        {
            VkCommandPoolCreateInfo ret = new VkCommandPoolCreateInfo();
            ret.sType = VkStructureType.CommandPoolCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandBufferAllocateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandPool commandPool;
        public VkCommandBufferLevel level;
        public uint commandBufferCount;
        public static VkCommandBufferAllocateInfo New()
        {
            VkCommandBufferAllocateInfo ret = new VkCommandBufferAllocateInfo();
            ret.sType = VkStructureType.CommandBufferAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandBufferInheritanceInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRenderPass renderPass;
        public uint subpass;
        public VkFramebuffer framebuffer;
        public VkBool32 occlusionQueryEnable;
        public VkQueryControlFlags queryFlags;
        public VkQueryPipelineStatisticFlags pipelineStatistics;
        public static VkCommandBufferInheritanceInfo New()
        {
            VkCommandBufferInheritanceInfo ret = new VkCommandBufferInheritanceInfo();
            ret.sType = VkStructureType.CommandBufferInheritanceInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCommandBufferBeginInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkCommandBufferUsageFlags flags;
        public VkCommandBufferInheritanceInfo* pInheritanceInfo;
        public static VkCommandBufferBeginInfo New()
        {
            VkCommandBufferBeginInfo ret = new VkCommandBufferBeginInfo();
            ret.sType = VkStructureType.CommandBufferBeginInfo;
            return ret;
        }
    }

    public unsafe partial struct VkRenderPassBeginInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRenderPass renderPass;
        public VkFramebuffer framebuffer;
        public VkRect2D renderArea;
        public uint clearValueCount;
        public VkClearValue* pClearValues;
        public static VkRenderPassBeginInfo New()
        {
            VkRenderPassBeginInfo ret = new VkRenderPassBeginInfo();
            ret.sType = VkStructureType.RenderPassBeginInfo;
            return ret;
        }
    }

    public unsafe partial struct VkClearDepthStencilValue
    {
        public float depth;
        public uint stencil;
    }

    public unsafe partial struct VkClearAttachment
    {
        public VkImageAspectFlags aspectMask;
        public uint colorAttachment;
        public VkClearValue clearValue;
    }

    public unsafe partial struct VkAttachmentDescription
    {
        public VkAttachmentDescriptionFlags flags;
        public VkFormat format;
        public VkSampleCountFlags samples;
        public VkAttachmentLoadOp loadOp;
        public VkAttachmentStoreOp storeOp;
        public VkAttachmentLoadOp stencilLoadOp;
        public VkAttachmentStoreOp stencilStoreOp;
        public VkImageLayout initialLayout;
        public VkImageLayout finalLayout;
    }

    public unsafe partial struct VkAttachmentReference
    {
        public uint attachment;
        public VkImageLayout layout;
    }

    public unsafe partial struct VkSubpassDescription
    {
        public uint flags;
        public VkPipelineBindPoint pipelineBindPoint;
        public uint inputAttachmentCount;
        public VkAttachmentReference* pInputAttachments;
        public uint colorAttachmentCount;
        public VkAttachmentReference* pColorAttachments;
        public VkAttachmentReference* pResolveAttachments;
        public VkAttachmentReference* pDepthStencilAttachment;
        public uint preserveAttachmentCount;
        public uint* pPreserveAttachments;
    }

    public unsafe partial struct VkSubpassDependency
    {
        public uint srcSubpass;
        public uint dstSubpass;
        public VkPipelineStageFlags srcStageMask;
        public VkPipelineStageFlags dstStageMask;
        public VkAccessFlags srcAccessMask;
        public VkAccessFlags dstAccessMask;
        public VkDependencyFlags dependencyFlags;
    }

    public unsafe partial struct VkRenderPassCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public uint attachmentCount;
        public VkAttachmentDescription* pAttachments;
        public uint subpassCount;
        public VkSubpassDescription* pSubpasses;
        public uint dependencyCount;
        public VkSubpassDependency* pDependencies;
        public static VkRenderPassCreateInfo New()
        {
            VkRenderPassCreateInfo ret = new VkRenderPassCreateInfo();
            ret.sType = VkStructureType.RenderPassCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkEventCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public static VkEventCreateInfo New()
        {
            VkEventCreateInfo ret = new VkEventCreateInfo();
            ret.sType = VkStructureType.EventCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkFenceCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public VkFenceCreateFlags flags;
        public static VkFenceCreateInfo New()
        {
            VkFenceCreateInfo ret = new VkFenceCreateInfo();
            ret.sType = VkStructureType.FenceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPhysicalDeviceFeatures
    {
        public VkBool32 robustBufferAccess;
        public VkBool32 fullDrawIndexUint32;
        public VkBool32 imageCubeArray;
        public VkBool32 independentBlend;
        public VkBool32 geometryShader;
        public VkBool32 tessellationShader;
        public VkBool32 sampleRateShading;
        public VkBool32 dualSrcBlend;
        public VkBool32 logicOp;
        public VkBool32 multiDrawIndirect;
        public VkBool32 drawIndirectFirstInstance;
        public VkBool32 depthClamp;
        public VkBool32 depthBiasClamp;
        public VkBool32 fillModeNonSolid;
        public VkBool32 depthBounds;
        public VkBool32 wideLines;
        public VkBool32 largePoints;
        public VkBool32 alphaToOne;
        public VkBool32 multiViewport;
        public VkBool32 samplerAnisotropy;
        public VkBool32 textureCompressionETC2;
        public VkBool32 textureCompressionASTC_LDR;
        public VkBool32 textureCompressionBC;
        public VkBool32 occlusionQueryPrecise;
        public VkBool32 pipelineStatisticsQuery;
        public VkBool32 vertexPipelineStoresAndAtomics;
        public VkBool32 fragmentStoresAndAtomics;
        public VkBool32 shaderTessellationAndGeometryPointSize;
        public VkBool32 shaderImageGatherExtended;
        public VkBool32 shaderStorageImageExtendedFormats;
        public VkBool32 shaderStorageImageMultisample;
        public VkBool32 shaderStorageImageReadWithoutFormat;
        public VkBool32 shaderStorageImageWriteWithoutFormat;
        public VkBool32 shaderUniformBufferArrayDynamicIndexing;
        public VkBool32 shaderSampledImageArrayDynamicIndexing;
        public VkBool32 shaderStorageBufferArrayDynamicIndexing;
        public VkBool32 shaderStorageImageArrayDynamicIndexing;
        public VkBool32 shaderClipDistance;
        public VkBool32 shaderCullDistance;
        public VkBool32 shaderFloat64;
        public VkBool32 shaderInt64;
        public VkBool32 shaderInt16;
        public VkBool32 shaderResourceResidency;
        public VkBool32 shaderResourceMinLod;
        public VkBool32 sparseBinding;
        public VkBool32 sparseResidencyBuffer;
        public VkBool32 sparseResidencyImage2D;
        public VkBool32 sparseResidencyImage3D;
        public VkBool32 sparseResidency2Samples;
        public VkBool32 sparseResidency4Samples;
        public VkBool32 sparseResidency8Samples;
        public VkBool32 sparseResidency16Samples;
        public VkBool32 sparseResidencyAliased;
        public VkBool32 variableMultisampleRate;
        public VkBool32 inheritedQueries;
    }

    public unsafe partial struct VkPhysicalDeviceSparseProperties
    {
        public VkBool32 residencyStandard2DBlockShape;
        public VkBool32 residencyStandard2DMultisampleBlockShape;
        public VkBool32 residencyStandard3DBlockShape;
        public VkBool32 residencyAlignedMipSize;
        public VkBool32 residencyNonResidentStrict;
    }

    public unsafe partial struct VkPhysicalDeviceLimits
    {
        public uint maxImageDimension1D;
        public uint maxImageDimension2D;
        public uint maxImageDimension3D;
        public uint maxImageDimensionCube;
        public uint maxImageArrayLayers;
        public uint maxTexelBufferElements;
        public uint maxUniformBufferRange;
        public uint maxStorageBufferRange;
        public uint maxPushConstantsSize;
        public uint maxMemoryAllocationCount;
        public uint maxSamplerAllocationCount;
        public ulong bufferImageGranularity;
        public ulong sparseAddressSpaceSize;
        public uint maxBoundDescriptorSets;
        public uint maxPerStageDescriptorSamplers;
        public uint maxPerStageDescriptorUniformBuffers;
        public uint maxPerStageDescriptorStorageBuffers;
        public uint maxPerStageDescriptorSampledImages;
        public uint maxPerStageDescriptorStorageImages;
        public uint maxPerStageDescriptorInputAttachments;
        public uint maxPerStageResources;
        public uint maxDescriptorSetSamplers;
        public uint maxDescriptorSetUniformBuffers;
        public uint maxDescriptorSetUniformBuffersDynamic;
        public uint maxDescriptorSetStorageBuffers;
        public uint maxDescriptorSetStorageBuffersDynamic;
        public uint maxDescriptorSetSampledImages;
        public uint maxDescriptorSetStorageImages;
        public uint maxDescriptorSetInputAttachments;
        public uint maxVertexInputAttributes;
        public uint maxVertexInputBindings;
        public uint maxVertexInputAttributeOffset;
        public uint maxVertexInputBindingStride;
        public uint maxVertexOutputComponents;
        public uint maxTessellationGenerationLevel;
        public uint maxTessellationPatchSize;
        public uint maxTessellationControlPerVertexInputComponents;
        public uint maxTessellationControlPerVertexOutputComponents;
        public uint maxTessellationControlPerPatchOutputComponents;
        public uint maxTessellationControlTotalOutputComponents;
        public uint maxTessellationEvaluationInputComponents;
        public uint maxTessellationEvaluationOutputComponents;
        public uint maxGeometryShaderInvocations;
        public uint maxGeometryInputComponents;
        public uint maxGeometryOutputComponents;
        public uint maxGeometryOutputVertices;
        public uint maxGeometryTotalOutputComponents;
        public uint maxFragmentInputComponents;
        public uint maxFragmentOutputAttachments;
        public uint maxFragmentDualSrcAttachments;
        public uint maxFragmentCombinedOutputResources;
        public uint maxComputeSharedMemorySize;
        public uint maxComputeWorkGroupCount_0;
        public uint maxComputeWorkGroupCount_1;
        public uint maxComputeWorkGroupCount_2;
        public uint maxComputeWorkGroupInvocations;
        public uint maxComputeWorkGroupSize_0;
        public uint maxComputeWorkGroupSize_1;
        public uint maxComputeWorkGroupSize_2;
        public uint subPixelPrecisionBits;
        public uint subTexelPrecisionBits;
        public uint mipmapPrecisionBits;
        public uint maxDrawIndexedIndexValue;
        public uint maxDrawIndirectCount;
        public float maxSamplerLodBias;
        public float maxSamplerAnisotropy;
        public uint maxViewports;
        public uint maxViewportDimensions_0;
        public uint maxViewportDimensions_1;
        public float viewportBoundsRange_0;
        public float viewportBoundsRange_1;
        public uint viewportSubPixelBits;
        public UIntPtr minMemoryMapAlignment;
        public ulong minTexelBufferOffsetAlignment;
        public ulong minUniformBufferOffsetAlignment;
        public ulong minStorageBufferOffsetAlignment;
        public int minTexelOffset;
        public uint maxTexelOffset;
        public int minTexelGatherOffset;
        public uint maxTexelGatherOffset;
        public float minInterpolationOffset;
        public float maxInterpolationOffset;
        public uint subPixelInterpolationOffsetBits;
        public uint maxFramebufferWidth;
        public uint maxFramebufferHeight;
        public uint maxFramebufferLayers;
        public VkSampleCountFlags framebufferColorSampleCounts;
        public VkSampleCountFlags framebufferDepthSampleCounts;
        public VkSampleCountFlags framebufferStencilSampleCounts;
        public VkSampleCountFlags framebufferNoAttachmentsSampleCounts;
        public uint maxColorAttachments;
        public VkSampleCountFlags sampledImageColorSampleCounts;
        public VkSampleCountFlags sampledImageIntegerSampleCounts;
        public VkSampleCountFlags sampledImageDepthSampleCounts;
        public VkSampleCountFlags sampledImageStencilSampleCounts;
        public VkSampleCountFlags storageImageSampleCounts;
        public uint maxSampleMaskWords;
        public VkBool32 timestampComputeAndGraphics;
        public float timestampPeriod;
        public uint maxClipDistances;
        public uint maxCullDistances;
        public uint maxCombinedClipAndCullDistances;
        public uint discreteQueuePriorities;
        public float pointSizeRange_0;
        public float pointSizeRange_1;
        public float lineWidthRange_0;
        public float lineWidthRange_1;
        public float pointSizeGranularity;
        public float lineWidthGranularity;
        public VkBool32 strictLines;
        public VkBool32 standardSampleLocations;
        public ulong optimalBufferCopyOffsetAlignment;
        public ulong optimalBufferCopyRowPitchAlignment;
        public ulong nonCoherentAtomSize;
    }

    public unsafe partial struct VkSemaphoreCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public static VkSemaphoreCreateInfo New()
        {
            VkSemaphoreCreateInfo ret = new VkSemaphoreCreateInfo();
            ret.sType = VkStructureType.SemaphoreCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkQueryPoolCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkQueryType queryType;
        public uint queryCount;
        public VkQueryPipelineStatisticFlags pipelineStatistics;
        public static VkQueryPoolCreateInfo New()
        {
            VkQueryPoolCreateInfo ret = new VkQueryPoolCreateInfo();
            ret.sType = VkStructureType.QueryPoolCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkFramebufferCreateInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkRenderPass renderPass;
        public uint attachmentCount;
        public VkImageView* pAttachments;
        public uint width;
        public uint height;
        public uint layers;
        public static VkFramebufferCreateInfo New()
        {
            VkFramebufferCreateInfo ret = new VkFramebufferCreateInfo();
            ret.sType = VkStructureType.FramebufferCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDrawIndirectCommand
    {
        public uint vertexCount;
        public uint instanceCount;
        public uint firstVertex;
        public uint firstInstance;
    }

    public unsafe partial struct VkDrawIndexedIndirectCommand
    {
        public uint indexCount;
        public uint instanceCount;
        public uint firstIndex;
        public int vertexOffset;
        public uint firstInstance;
    }

    public unsafe partial struct VkDispatchIndirectCommand
    {
        public uint x;
        public uint y;
        public uint z;
    }

    public unsafe partial struct VkSubmitInfo
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public VkPipelineStageFlags* pWaitDstStageMask;
        public uint commandBufferCount;
        public VkCommandBuffer* pCommandBuffers;
        public uint signalSemaphoreCount;
        public VkSemaphore* pSignalSemaphores;
        public static VkSubmitInfo New()
        {
            VkSubmitInfo ret = new VkSubmitInfo();
            ret.sType = VkStructureType.SubmitInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayPropertiesKHR
    {
        public VkDisplayKHR display;
        public byte* displayName;
        public VkExtent2D physicalDimensions;
        public VkExtent2D physicalResolution;
        public VkSurfaceTransformFlagsKHR supportedTransforms;
        public VkBool32 planeReorderPossible;
        public VkBool32 persistentContent;
    }

    public unsafe partial struct VkDisplayPlanePropertiesKHR
    {
        public VkDisplayKHR currentDisplay;
        public uint currentStackIndex;
    }

    public unsafe partial struct VkDisplayModeParametersKHR
    {
        public VkExtent2D visibleRegion;
        public uint refreshRate;
    }

    public unsafe partial struct VkDisplayModePropertiesKHR
    {
        public VkDisplayModeKHR displayMode;
        public VkDisplayModeParametersKHR parameters;
    }

    public unsafe partial struct VkDisplayModeCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkDisplayModeParametersKHR parameters;
        public static VkDisplayModeCreateInfoKHR New()
        {
            VkDisplayModeCreateInfoKHR ret = new VkDisplayModeCreateInfoKHR();
            ret.sType = VkStructureType.DisplayModeCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayPlaneCapabilitiesKHR
    {
        public VkDisplayPlaneAlphaFlagsKHR supportedAlpha;
        public VkOffset2D minSrcPosition;
        public VkOffset2D maxSrcPosition;
        public VkExtent2D minSrcExtent;
        public VkExtent2D maxSrcExtent;
        public VkOffset2D minDstPosition;
        public VkOffset2D maxDstPosition;
        public VkExtent2D minDstExtent;
        public VkExtent2D maxDstExtent;
    }

    public unsafe partial struct VkDisplaySurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkDisplayModeKHR displayMode;
        public uint planeIndex;
        public uint planeStackIndex;
        public VkSurfaceTransformFlagsKHR transform;
        public float globalAlpha;
        public VkDisplayPlaneAlphaFlagsKHR alphaMode;
        public VkExtent2D imageExtent;
        public static VkDisplaySurfaceCreateInfoKHR New()
        {
            VkDisplaySurfaceCreateInfoKHR ret = new VkDisplaySurfaceCreateInfoKHR();
            ret.sType = VkStructureType.DisplaySurfaceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDisplayPresentInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRect2D srcRect;
        public VkRect2D dstRect;
        public VkBool32 persistent;
        public static VkDisplayPresentInfoKHR New()
        {
            VkDisplayPresentInfoKHR ret = new VkDisplayPresentInfoKHR();
            ret.sType = VkStructureType.DisplayPresentInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSurfaceCapabilitiesKHR
    {
        public uint minImageCount;
        public uint maxImageCount;
        public VkExtent2D currentExtent;
        public VkExtent2D minImageExtent;
        public VkExtent2D maxImageExtent;
        public uint maxImageArrayLayers;
        public VkSurfaceTransformFlagsKHR supportedTransforms;
        public VkSurfaceTransformFlagsKHR currentTransform;
        public VkCompositeAlphaFlagsKHR supportedCompositeAlpha;
        public VkImageUsageFlags supportedUsageFlags;
    }

    public unsafe partial struct VkAndroidSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Android.ANativeWindow* window;
        public static VkAndroidSurfaceCreateInfoKHR New()
        {
            VkAndroidSurfaceCreateInfoKHR ret = new VkAndroidSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.AndroidSurfaceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkMirSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Mir.MirConnection* connection;
        public Mir.MirSurface* mirSurface;
        public static VkMirSurfaceCreateInfoKHR New()
        {
            VkMirSurfaceCreateInfoKHR ret = new VkMirSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.MirSurfaceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkWaylandSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Wayland.wl_display* display;
        public Wayland.wl_surface* surface;
        public static VkWaylandSurfaceCreateInfoKHR New()
        {
            VkWaylandSurfaceCreateInfoKHR ret = new VkWaylandSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.WaylandSurfaceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkWin32SurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Win32.HINSTANCE hinstance;
        public Win32.HWND hwnd;
        public static VkWin32SurfaceCreateInfoKHR New()
        {
            VkWin32SurfaceCreateInfoKHR ret = new VkWin32SurfaceCreateInfoKHR();
            ret.sType = VkStructureType.Win32SurfaceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkXlibSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Xlib.Display* dpy;
        public Xlib.Window window;
        public static VkXlibSurfaceCreateInfoKHR New()
        {
            VkXlibSurfaceCreateInfoKHR ret = new VkXlibSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.XlibSurfaceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkXcbSurfaceCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public Xcb.xcb_connection_t* connection;
        public Xcb.xcb_window_t window;
        public static VkXcbSurfaceCreateInfoKHR New()
        {
            VkXcbSurfaceCreateInfoKHR ret = new VkXcbSurfaceCreateInfoKHR();
            ret.sType = VkStructureType.XcbSurfaceCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkSurfaceFormatKHR
    {
        public VkFormat format;
        public VkColorSpaceKHR colorSpace;
    }

    public unsafe partial struct VkSwapchainCreateInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint flags;
        public VkSurfaceKHR surface;
        public uint minImageCount;
        public VkFormat imageFormat;
        public VkColorSpaceKHR imageColorSpace;
        public VkExtent2D imageExtent;
        public uint imageArrayLayers;
        public VkImageUsageFlags imageUsage;
        public VkSharingMode imageSharingMode;
        public uint queueFamilyIndexCount;
        public uint* pQueueFamilyIndices;
        public VkSurfaceTransformFlagsKHR preTransform;
        public VkCompositeAlphaFlagsKHR compositeAlpha;
        public VkPresentModeKHR presentMode;
        public VkBool32 clipped;
        public VkSwapchainKHR oldSwapchain;
        public static VkSwapchainCreateInfoKHR New()
        {
            VkSwapchainCreateInfoKHR ret = new VkSwapchainCreateInfoKHR();
            ret.sType = VkStructureType.SwapchainCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkPresentInfoKHR
    {
        public VkStructureType sType;
        public void* pNext;
        public uint waitSemaphoreCount;
        public VkSemaphore* pWaitSemaphores;
        public uint swapchainCount;
        public VkSwapchainKHR* pSwapchains;
        public uint* pImageIndices;
        public VkResult* pResults;
        public static VkPresentInfoKHR New()
        {
            VkPresentInfoKHR ret = new VkPresentInfoKHR();
            ret.sType = VkStructureType.PresentInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDebugReportCallbackCreateInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportFlagsEXT flags;
        public IntPtr pfnCallback;
        public void* pUserData;
        public static VkDebugReportCallbackCreateInfoEXT New()
        {
            VkDebugReportCallbackCreateInfoEXT ret = new VkDebugReportCallbackCreateInfoEXT();
            ret.sType = VkStructureType.DebugReportCallbackCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkValidationFlagsEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public uint disabledValidationCheckCount;
        public VkValidationCheckEXT* pDisabledValidationChecks;
    }

    public unsafe partial struct VkPipelineRasterizationStateRasterizationOrderAMD
    {
        public VkStructureType sType;
        public void* pNext;
        public VkRasterizationOrderAMD rasterizationOrder;
        public static VkPipelineRasterizationStateRasterizationOrderAMD New()
        {
            VkPipelineRasterizationStateRasterizationOrderAMD ret = new VkPipelineRasterizationStateRasterizationOrderAMD();
            ret.sType = VkStructureType.PipelineRasterizationStateRasterizationOrder;
            return ret;
        }
    }

    public unsafe partial struct VkDebugMarkerObjectNameInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportObjectTypeEXT objectType;
        public ulong @object;
        public byte* pObjectName;
        public static VkDebugMarkerObjectNameInfoEXT New()
        {
            VkDebugMarkerObjectNameInfoEXT ret = new VkDebugMarkerObjectNameInfoEXT();
            ret.sType = VkStructureType.DebugMarkerObjectNameInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDebugMarkerObjectTagInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public VkDebugReportObjectTypeEXT objectType;
        public ulong @object;
        public ulong tagName;
        public UIntPtr tagSize;
        public void* pTag;
        public static VkDebugMarkerObjectTagInfoEXT New()
        {
            VkDebugMarkerObjectTagInfoEXT ret = new VkDebugMarkerObjectTagInfoEXT();
            ret.sType = VkStructureType.DebugMarkerObjectTagInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDebugMarkerMarkerInfoEXT
    {
        public VkStructureType sType;
        public void* pNext;
        public byte* pMarkerName;
        public float color_0;
        public float color_1;
        public float color_2;
        public float color_3;
        public static VkDebugMarkerMarkerInfoEXT New()
        {
            VkDebugMarkerMarkerInfoEXT ret = new VkDebugMarkerMarkerInfoEXT();
            ret.sType = VkStructureType.DebugMarkerMarkerInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDedicatedAllocationImageCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 dedicatedAllocation;
        public static VkDedicatedAllocationImageCreateInfoNV New()
        {
            VkDedicatedAllocationImageCreateInfoNV ret = new VkDedicatedAllocationImageCreateInfoNV();
            ret.sType = VkStructureType.DedicatedAllocationImageCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDedicatedAllocationBufferCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 dedicatedAllocation;
        public static VkDedicatedAllocationBufferCreateInfoNV New()
        {
            VkDedicatedAllocationBufferCreateInfoNV ret = new VkDedicatedAllocationBufferCreateInfoNV();
            ret.sType = VkStructureType.DedicatedAllocationBufferCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDedicatedAllocationMemoryAllocateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkImage image;
        public VkBuffer buffer;
        public static VkDedicatedAllocationMemoryAllocateInfoNV New()
        {
            VkDedicatedAllocationMemoryAllocateInfoNV ret = new VkDedicatedAllocationMemoryAllocateInfoNV();
            ret.sType = VkStructureType.DedicatedAllocationMemoryAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkExternalImageFormatPropertiesNV
    {
        public VkImageFormatProperties imageFormatProperties;
        public VkExternalMemoryFeatureFlagsNV externalMemoryFeatures;
        public VkExternalMemoryHandleTypeFlagsNV exportFromImportedHandleTypes;
        public VkExternalMemoryHandleTypeFlagsNV compatibleHandleTypes;
    }

    public unsafe partial struct VkExternalMemoryImageCreateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleTypes;
        public static VkExternalMemoryImageCreateInfoNV New()
        {
            VkExternalMemoryImageCreateInfoNV ret = new VkExternalMemoryImageCreateInfoNV();
            ret.sType = VkStructureType.ExternalMemoryImageCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkExportMemoryAllocateInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleTypes;
        public static VkExportMemoryAllocateInfoNV New()
        {
            VkExportMemoryAllocateInfoNV ret = new VkExportMemoryAllocateInfoNV();
            ret.sType = VkStructureType.ExportMemoryAllocateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkImportMemoryWin32HandleInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public VkExternalMemoryHandleTypeFlagsNV handleType;
        public Win32.HANDLE handle;
        public static VkImportMemoryWin32HandleInfoNV New()
        {
            VkImportMemoryWin32HandleInfoNV ret = new VkImportMemoryWin32HandleInfoNV();
            ret.sType = VkStructureType.ImportMemoryWin32HandleInfo;
            return ret;
        }
    }

    public unsafe partial struct VkExportMemoryWin32HandleInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public Win32.SECURITY_ATTRIBUTES* pAttributes;
        public uint dwAccess;
        public static VkExportMemoryWin32HandleInfoNV New()
        {
            VkExportMemoryWin32HandleInfoNV ret = new VkExportMemoryWin32HandleInfoNV();
            ret.sType = VkStructureType.ExportMemoryWin32HandleInfo;
            return ret;
        }
    }

    public unsafe partial struct VkWin32KeyedMutexAcquireReleaseInfoNV
    {
        public VkStructureType sType;
        public void* pNext;
        public uint acquireCount;
        public VkDeviceMemory* pAcquireSyncs;
        public ulong* pAcquireKeys;
        public uint* pAcquireTimeoutMilliseconds;
        public uint releaseCount;
        public VkDeviceMemory* pReleaseSyncs;
        public ulong* pReleaseKeys;
        public static VkWin32KeyedMutexAcquireReleaseInfoNV New()
        {
            VkWin32KeyedMutexAcquireReleaseInfoNV ret = new VkWin32KeyedMutexAcquireReleaseInfoNV();
            ret.sType = VkStructureType.Win32KeyedMutexAcquireReleaseInfo;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGeneratedCommandsFeaturesNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkBool32 computeBindingPointSupport;
        public static VkDeviceGeneratedCommandsFeaturesNVX New()
        {
            VkDeviceGeneratedCommandsFeaturesNVX ret = new VkDeviceGeneratedCommandsFeaturesNVX();
            ret.sType = VkStructureType.DeviceGeneratedCommandsFeatures;
            return ret;
        }
    }

    public unsafe partial struct VkDeviceGeneratedCommandsLimitsNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint maxIndirectCommandsLayoutTokenCount;
        public uint maxObjectEntryCounts;
        public uint minSequenceCountBufferOffsetAlignment;
        public uint minSequenceIndexBufferOffsetAlignment;
        public uint minCommandsTokenBufferOffsetAlignment;
        public static VkDeviceGeneratedCommandsLimitsNVX New()
        {
            VkDeviceGeneratedCommandsLimitsNVX ret = new VkDeviceGeneratedCommandsLimitsNVX();
            ret.sType = VkStructureType.DeviceGeneratedCommandsLimits;
            return ret;
        }
    }

    public unsafe partial struct VkIndirectCommandsTokenNVX
    {
        public VkIndirectCommandsTokenTypeNVX tokenType;
        public VkBuffer buffer;
        public ulong offset;
    }

    public unsafe partial struct VkIndirectCommandsLayoutTokenNVX
    {
        public VkIndirectCommandsTokenTypeNVX tokenType;
        public uint bindingUnit;
        public uint dynamicCount;
        public uint divisor;
    }

    public unsafe partial struct VkIndirectCommandsLayoutCreateInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkPipelineBindPoint pipelineBindPoint;
        public VkIndirectCommandsLayoutUsageFlagsNVX flags;
        public uint tokenCount;
        public VkIndirectCommandsLayoutTokenNVX* pTokens;
        public static VkIndirectCommandsLayoutCreateInfoNVX New()
        {
            VkIndirectCommandsLayoutCreateInfoNVX ret = new VkIndirectCommandsLayoutCreateInfoNVX();
            ret.sType = VkStructureType.IndirectCommandsLayoutCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCmdProcessCommandsInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkObjectTableNVX objectTable;
        public VkIndirectCommandsLayoutNVX indirectCommandsLayout;
        public uint indirectCommandsTokenCount;
        public VkIndirectCommandsTokenNVX* pIndirectCommandsTokens;
        public uint maxSequencesCount;
        public VkCommandBuffer targetCommandBuffer;
        public VkBuffer sequencesCountBuffer;
        public ulong sequencesCountOffset;
        public VkBuffer sequencesIndexBuffer;
        public ulong sequencesIndexOffset;
        public static VkCmdProcessCommandsInfoNVX New()
        {
            VkCmdProcessCommandsInfoNVX ret = new VkCmdProcessCommandsInfoNVX();
            ret.sType = VkStructureType.CmdProcessCommandsInfo;
            return ret;
        }
    }

    public unsafe partial struct VkCmdReserveSpaceForCommandsInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public VkObjectTableNVX objectTable;
        public VkIndirectCommandsLayoutNVX indirectCommandsLayout;
        public uint maxSequencesCount;
        public static VkCmdReserveSpaceForCommandsInfoNVX New()
        {
            VkCmdReserveSpaceForCommandsInfoNVX ret = new VkCmdReserveSpaceForCommandsInfoNVX();
            ret.sType = VkStructureType.CmdReserveSpaceForCommandsInfo;
            return ret;
        }
    }

    public unsafe partial struct VkObjectTableCreateInfoNVX
    {
        public VkStructureType sType;
        public void* pNext;
        public uint objectCount;
        public VkObjectEntryTypeNVX* pObjectEntryTypes;
        public uint* pObjectEntryCounts;
        public VkObjectEntryUsageFlagsNVX* pObjectEntryUsageFlags;
        public uint maxUniformBuffersPerDescriptor;
        public uint maxStorageBuffersPerDescriptor;
        public uint maxStorageImagesPerDescriptor;
        public uint maxSampledImagesPerDescriptor;
        public uint maxPipelineLayouts;
        public static VkObjectTableCreateInfoNVX New()
        {
            VkObjectTableCreateInfoNVX ret = new VkObjectTableCreateInfoNVX();
            ret.sType = VkStructureType.ObjectTableCreateInfo;
            return ret;
        }
    }

    public unsafe partial struct VkObjectTableEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
    }

    public unsafe partial struct VkObjectTablePipelineEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkPipeline pipeline;
    }

    public unsafe partial struct VkObjectTableDescriptorSetEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkPipelineLayout pipelineLayout;
        public VkDescriptorSet descriptorSet;
    }

    public unsafe partial struct VkObjectTableVertexBufferEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkBuffer buffer;
    }

    public unsafe partial struct VkObjectTableIndexBufferEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkBuffer buffer;
    }

    public unsafe partial struct VkObjectTablePushConstantEntryNVX
    {
        public VkObjectEntryTypeNVX type;
        public VkObjectEntryUsageFlagsNVX flags;
        public VkPipelineLayout pipelineLayout;
        public VkShaderStageFlags stageFlags;
    }
}
#endif // CALLI_STUBS
