using System;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Collections;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class TriangleFromScratch
    {
        private VkInstance _instance;
        private VkPhysicalDevice _physicalDevice;
        private VkPhysicalDeviceProperties _physicalDeviceProperties;
        private VkPhysicalDeviceFeatures _physicalDeviceFeatures;
        private uint _graphicsQueueIndex;

        public static void Main()
        {
            new TriangleFromScratch().RunSample();
        }

        private void RunSample()
        {
            CreateInstance();
            CreatePhysicalDevice();
            CreateLogicalDevice();
        }

        private void CreateInstance()
        {
            VkInstanceCreateInfo instanceCreateInfo = VkInstanceCreateInfo.New();
            VkApplicationInfo appInfo = VkApplicationInfo.New();
            appInfo.pApplicationName = Strings.AppName;
            appInfo.pEngineName = Strings.EngineName;
            appInfo.apiVersion = new Version(1, 0, 0);
            appInfo.engineVersion = new Version(1, 0, 0);
            appInfo.apiVersion = new Version(1, 0, 0);
            instanceCreateInfo.pApplicationInfo = &appInfo;
            RawList<IntPtr> instanceLayers = new RawList<IntPtr>();
            RawList<IntPtr> instanceExtensions = new RawList<IntPtr>();
            instanceExtensions.Add(Strings.VK_KHR_SURFACE_EXTENSION_NAME);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                instanceExtensions.Add(Strings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                instanceExtensions.Add(Strings.VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            bool debug = true;
            if (debug)
            {
                instanceExtensions.Add(Strings.VK_EXT_DEBUG_REPORT_EXTENSION_NAME);
                instanceLayers.Add(Strings.StandardValidationLayerName);
            }

            fixed (IntPtr* extensionsBase = &instanceExtensions.Items[0])
            fixed (IntPtr* layersBase = &instanceLayers.Items[0])
            {
                instanceCreateInfo.enabledExtensionCount = instanceExtensions.Count;
                instanceCreateInfo.ppEnabledExtensionNames = (byte**)extensionsBase;
                instanceCreateInfo.enabledLayerCount = instanceLayers.Count;
                instanceCreateInfo.ppEnabledLayerNames = (byte**)(layersBase);
                CheckResult(vkCreateInstance(ref instanceCreateInfo, null, out _instance));
            }
        }

        private unsafe void CreatePhysicalDevice()
        {
            uint deviceCount = 0;
            vkEnumeratePhysicalDevices(_instance, ref deviceCount, null);
            if (deviceCount == 0)
            {
                throw new InvalidOperationException("No physical devices exist.");
            }

            vkEnumeratePhysicalDevices(_instance, ref deviceCount, ref _physicalDevice);
            vkGetPhysicalDeviceProperties(_physicalDevice, out _physicalDeviceProperties);
            string deviceName;
            fixed (byte* utf8NamePtr = _physicalDeviceProperties.deviceName)
            {
                deviceName = Encoding.UTF8.GetString(utf8NamePtr, (int)MaxPhysicalDeviceNameSize);
            }

            vkGetPhysicalDeviceFeatures(_physicalDevice, out _physicalDeviceFeatures);

            Console.WriteLine($"Using device: {deviceName}");
        }

        private unsafe void CreateLogicalDevice()
        {
            GetQueues();

            VkDeviceQueueCreateInfo queueCreateInfo = VkDeviceQueueCreateInfo.New();
            queueCreateInfo.queueFamilyIndex = _graphicsQueueIndex;
            queueCreateInfo.queueCount = 1;
            float priority = 1f;
            queueCreateInfo.pQueuePriorities = &priority;

            VkPhysicalDeviceFeatures deviceFeatures = new VkPhysicalDeviceFeatures();

            byte* layerNames = Strings.StandardValidationLayerName;
            VkDeviceCreateInfo deviceCreateInfo = VkDeviceCreateInfo.New();
            deviceCreateInfo.pQueueCreateInfos = &queueCreateInfo;
            deviceCreateInfo.queueCreateInfoCount = 1;
            deviceCreateInfo.pEnabledFeatures = &deviceFeatures;
            deviceCreateInfo.enabledLayerCount = 1;
            deviceCreateInfo.ppEnabledLayerNames = &layerNames;


        }

        private void GetQueues()
        {
            uint queueFamilyCount = 0;
            vkGetPhysicalDeviceQueueFamilyProperties(_physicalDevice, ref queueFamilyCount, null);
            VkQueueFamilyProperties[] qfp = new VkQueueFamilyProperties[queueFamilyCount];
            vkGetPhysicalDeviceQueueFamilyProperties(_physicalDevice, ref queueFamilyCount, out qfp[0]);

            for (uint i = 0; i < qfp.Length; i++)
            {
                if ((qfp[i].queueFlags & VkQueueFlags.Graphics) != 0)
                {
                    _graphicsQueueIndex = i;
                }
            }
        }

        private static void CheckResult(VkResult result)
        {
            if (result != VkResult.Success)
            {
                Console.WriteLine($"Vulkan call was not successful: {result}");
            }
        }
    }
}