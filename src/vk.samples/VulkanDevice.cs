using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class VulkanDevice
    {
        public VkPhysicalDevice PhysicalDevice { get; private set; }
        public VkPhysicalDeviceProperties Properties { get; private set; }
        public VkPhysicalDeviceFeatures Features { get; private set; }
        public VkPhysicalDeviceMemoryProperties MemoryProperties { get; private set; }
        public NativeList<VkQueueFamilyProperties> QueueFamilyProperties { get; } = new NativeList<VkQueueFamilyProperties>();
        public List<string> SuppertedExcentions { get; } = new List<string>();
        public VkDevice LogicalDevice { get; private set; }
        public VkCommandPool CommandPool { get; private set; }
        public bool EnableDebugMarkers { get; internal set; }

        public QueueFamilyIndices QFIndices;

        public VulkanDevice(VkPhysicalDevice physicalDevice)
        {
            Debug.Assert(physicalDevice.Handle != IntPtr.Zero);
            PhysicalDevice = physicalDevice;

            // Store Properties features, limits and properties of the physical device for later use
            // Device properties also contain limits and sparse properties
            VkPhysicalDeviceProperties properties;
            vkGetPhysicalDeviceProperties(physicalDevice, &properties);
            //Properties = properties;
            // Features should be checked by the examples before using them
            VkPhysicalDeviceFeatures features;
            vkGetPhysicalDeviceFeatures(physicalDevice, &features);
            Features = features;
            // Memory properties are used regularly for creating all kinds of buffers
            VkPhysicalDeviceMemoryProperties memoryProperties;
            vkGetPhysicalDeviceMemoryProperties(physicalDevice, &memoryProperties);
            MemoryProperties = memoryProperties;
            // Queue family properties, used for setting up requested queues upon device creation
            uint queueFamilyCount;
            vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, null);
            Debug.Assert(queueFamilyCount > 0);
            QueueFamilyProperties.Resize(queueFamilyCount);
            vkGetPhysicalDeviceQueueFamilyProperties(
                physicalDevice,
                &queueFamilyCount,
                (VkQueueFamilyProperties*)QueueFamilyProperties.Data.ToPointer());
            QueueFamilyProperties.Count = queueFamilyCount;

            // Get list of supported extensions
            uint extCount = 0;
            vkEnumerateDeviceExtensionProperties(physicalDevice, null, &extCount, null);
            if (extCount > 0)
            {
                VkExtensionProperties* extensions = stackalloc VkExtensionProperties[(int)extCount];
                if (vkEnumerateDeviceExtensionProperties(physicalDevice, null, &extCount, extensions) == VkResult.Success)
                {
                    for (uint i = 0; i < extCount; i++)
                    {
                        var ext = extensions[i];
                        // supportedExtensions.push_back(ext.extensionName);
                        // TODO: fixed-length char arrays are not being parsed correctly.
                    }
                }
            }
        }


        public VkResult CreateLogicalDevice(
            VkPhysicalDeviceFeatures enabledFeatures,
            NativeList<IntPtr> enabledExtensions,
            bool useSwapChain = true,
            VkQueueFlagBits requestedQueueTypes = VkQueueFlagBits.Graphics | VkQueueFlagBits.Compute)
        {
            // Desired queues need to be requested upon logical device creation
            // Due to differing queue family configurations of Vulkan implementations this can be a bit tricky, especially if the application
            // requests different queue types

            using (NativeList<VkDeviceQueueCreateInfo> queueCreateInfos = new NativeList<VkDeviceQueueCreateInfo>())
            {
                float defaultQueuePriority = 0.0f;

                // Graphics queue
                if ((requestedQueueTypes & VkQueueFlagBits.Graphics) != 0)
                {
                    QFIndices.Graphics = GetQueueFamilyIndex(VkQueueFlagBits.Graphics);
                    VkDeviceQueueCreateInfo queueInfo = new VkDeviceQueueCreateInfo();
                    queueInfo.sType = VkStructureType.DeviceQueueCreateInfo;
                    queueInfo.queueFamilyIndex = QFIndices.Graphics;
                    queueInfo.queueCount = 1;
                    queueInfo.pQueuePriorities = &defaultQueuePriority;
                    queueCreateInfos.Add(queueInfo);
                }
                else
                {
                    QFIndices.Graphics = (uint)Hacks.VK_NULL_HANDLE;
                }

                // Dedicated compute queue
                if ((requestedQueueTypes & VkQueueFlagBits.Compute) != 0)
                {
                    QFIndices.Compute = GetQueueFamilyIndex(VkQueueFlagBits.Compute);
                    if (QFIndices.Compute != QFIndices.Graphics)
                    {
                        // If compute family index differs, we need an additional queue create info for the compute queue
                        VkDeviceQueueCreateInfo queueInfo = new VkDeviceQueueCreateInfo();
                        queueInfo.sType = VkStructureType.DeviceQueueCreateInfo;
                        queueInfo.queueFamilyIndex = QFIndices.Compute;
                        queueInfo.queueCount = 1;
                        queueInfo.pQueuePriorities = &defaultQueuePriority;
                        queueCreateInfos.Add(queueInfo);
                    }
                }
                else
                {
                    // Else we use the same queue
                    QFIndices.Compute = QFIndices.Graphics;
                }

                // Dedicated transfer queue
                if ((requestedQueueTypes & VkQueueFlagBits.Transfer) != 0)
                {
                    QFIndices.Transfer = GetQueueFamilyIndex(VkQueueFlagBits.Transfer);
                    if (QFIndices.Transfer != QFIndices.Graphics && QFIndices.Transfer != QFIndices.Compute)
                    {
                        // If compute family index differs, we need an additional queue create info for the transfer queue
                        VkDeviceQueueCreateInfo queueInfo = new VkDeviceQueueCreateInfo();
                        queueInfo.sType = VkStructureType.DeviceQueueCreateInfo;
                        queueInfo.queueFamilyIndex = QFIndices.Transfer;
                        queueInfo.queueCount = 1;
                        queueInfo.pQueuePriorities = &defaultQueuePriority;
                        queueCreateInfos.Add(queueInfo);
                    }
                }
                else
                {
                    // Else we use the same queue
                    QFIndices.Transfer = QFIndices.Graphics;
                }

                // Create the logical device representation
                using (NativeList<IntPtr> deviceExtensions = new NativeList<IntPtr>(enabledExtensions))
                {
                    if (useSwapChain)
                    {
                        // If the device will be used for presenting to a display via a swapchain we need to request the swapchain extension
                        deviceExtensions.Add(Strings.VK_KHR_SWAPCHAIN_EXTENSION_NAME);
                    }

                    VkDeviceCreateInfo deviceCreateInfo = new VkDeviceCreateInfo();
                    deviceCreateInfo.sType = VkStructureType.DeviceCreateInfo;
                    deviceCreateInfo.queueCreateInfoCount = queueCreateInfos.Count;
                    deviceCreateInfo.pQueueCreateInfos = (VkDeviceQueueCreateInfo*)queueCreateInfos.Data.ToPointer();
                    deviceCreateInfo.pEnabledFeatures = &enabledFeatures;

                    if (deviceExtensions.Count > 0)
                    {
                        deviceCreateInfo.enabledExtensionCount = deviceExtensions.Count;
                        deviceCreateInfo.ppEnabledExtensionNames = (byte**)deviceExtensions.Data.ToPointer();
                    }

                    VkDevice logicalDevice;
                    VkResult result = vkCreateDevice(PhysicalDevice, &deviceCreateInfo, null, &logicalDevice);
                    LogicalDevice = logicalDevice;

                    if (result == VkResult.Success)
                    {
                        // Create a default command pool for graphics command buffers
                        CommandPool = CreateCommandPool(QFIndices.Graphics);
                    }

                    return result;
                }
            }
        }

        private uint GetQueueFamilyIndex(VkQueueFlagBits queueFlags)
        {
            // Dedicated queue for compute
            // Try to find a queue family index that supports compute but not graphics
            if ((queueFlags & VkQueueFlagBits.Compute) != 0)
            {
                for (uint i = 0; i < QueueFamilyProperties.Count; i++)
                {
                    if (((QueueFamilyProperties[i].queueFlags & (uint)queueFlags) != 0)
                        && (((VkQueueFlagBits)QueueFamilyProperties[i].queueFlags & VkQueueFlagBits.Graphics) == 0))
                    {
                        return i;
                    }
                }
            }

            // Dedicated queue for transfer
            // Try to find a queue family index that supports transfer but not graphics and compute
            if ((queueFlags & VkQueueFlagBits.Transfer) != 0)
            {
                for (uint i = 0; i < QueueFamilyProperties.Count; i++)
                {
                    if (((QueueFamilyProperties[i].queueFlags & (uint)queueFlags) != 0)
                        && (((VkQueueFlagBits)QueueFamilyProperties[i].queueFlags & VkQueueFlagBits.Graphics) == 0)
                        && (((VkQueueFlagBits)QueueFamilyProperties[i].queueFlags & VkQueueFlagBits.Compute) == 0))
                    {
                        return i;
                    }
                }
            }

            // For other queue types or if no separate compute queue is present, return the first one to support the requested flags
            for (uint i = 0; i < QueueFamilyProperties.Count; i++)
            {
                if ((QueueFamilyProperties[i].queueFlags & (uint)queueFlags) != 0)
                {
                    return i;
                }
            }

            throw new InvalidOperationException("Could not find a matching queue family index");
        }

        private VkCommandPool CreateCommandPool(
            uint queueFamilyIndex,
            VkCommandPoolCreateFlagBits createFlags = VkCommandPoolCreateFlagBits.ResetCommandBuffer)
        {
            VkCommandPoolCreateInfo cmdPoolInfo = new VkCommandPoolCreateInfo();
            cmdPoolInfo.sType = VkStructureType.CommandPoolCreateInfo;
            cmdPoolInfo.queueFamilyIndex = queueFamilyIndex;
            cmdPoolInfo.flags = (uint)createFlags;
            VkCommandPool cmdPool;
            Util.CheckResult(vkCreateCommandPool(LogicalDevice, &cmdPoolInfo, null, &cmdPool));
            return cmdPool;
        }

        /**
* Get the index of a memory type that has all the requested property bits set
*
* @param typeBits Bitmask with bits set for each memory type supported by the resource to request for (from VkMemoryRequirements)
* @param properties Bitmask of properties for the memory type to request
* @param (Optional) memTypeFound Pointer to a bool that is set to true if a matching memory type has been found
* 
* @return Index of the requested memory type
*
* @throw Throws an exception if memTypeFound is null and no memory type could be found that supports the requested properties
*/
        public uint getMemoryType(uint typeBits, VkMemoryPropertyFlagBits properties, uint* memTypeFound = null)
        {
            for (uint i = 0; i < MemoryProperties.memoryTypeCount; i++)
            {
                if ((typeBits & 1) == 1)
                {
                    if ((MemoryProperties.GetMemoryType(i).propertyFlags & (uint)properties) == (uint)properties)
                    {
                        if (memTypeFound != null)
                        {
                            *memTypeFound = True;
                        }
                        return i;
                    }
                }
                typeBits >>= 1;
            }

            if (memTypeFound != null)
            {
                *memTypeFound = False;
                return 0;
            }
            else
            {
                throw new InvalidOperationException("Could not find a matching memory type");
            }
        }

        public struct QueueFamilyIndices
        {
            public uint Graphics;
            public uint Compute;
            public uint Transfer;
        }
    }
}
