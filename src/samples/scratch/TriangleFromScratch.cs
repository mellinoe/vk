using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.CompilerServices;
using Veldrid.Collections;
using Veldrid.Sdl2;
using Vulkan;

using static Veldrid.Sdl2.Sdl2Native;
using static Vulkan.VulkanNative;
using System.Linq;

namespace Vk.Samples
{
    public unsafe class TriangleFromScratch
    {
        private VkInstance _instance;
        private VkPhysicalDevice _physicalDevice;
        private VkPhysicalDeviceProperties _physicalDeviceProperties;
        private VkPhysicalDeviceFeatures _physicalDeviceFeatures;
        private uint _graphicsQueueIndex;
        private uint _presentQueueIndex;
        private VkDevice _device;
        private VkQueue _graphicsQueue;
        private VkQueue _presentQueue;
        private VkSurfaceKHR _surface;

        // Swapchain stuff
        private RawList<VkImage> _scImages = new RawList<VkImage>();
        private RawList<VkImageView> _scImageViews = new RawList<VkImageView>();
        private VkSwapchainKHR _swapchain;
        private VkFormat _swapchainImageFormat;
        private VkExtent2D _swapchainExtent;

        private Sdl2Window _window;

        public static void Main()
        {
            new TriangleFromScratch().RunSample();
        }

        private void RunSample()
        {
            CreateInstance();
            CreatePlatformWindow();
            CreateSurface();
            CreatePhysicalDevice();
            CreateLogicalDevice();
            CreateSwapchain();
            CreateImageViews();
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
            GetQueueFamilyIndices();

            HashSet<uint> familyIndices = new HashSet<uint> { _graphicsQueueIndex, _presentQueueIndex };
            RawList<VkDeviceQueueCreateInfo> queueCreateInfos = new RawList<VkDeviceQueueCreateInfo>();

            foreach (uint index in familyIndices)
            {
                VkDeviceQueueCreateInfo queueCreateInfo = VkDeviceQueueCreateInfo.New();
                queueCreateInfo.queueFamilyIndex = _graphicsQueueIndex;
                queueCreateInfo.queueCount = 1;
                float priority = 1f;
                queueCreateInfo.pQueuePriorities = &priority;
                queueCreateInfos.Add(queueCreateInfo);
            }

            VkPhysicalDeviceFeatures deviceFeatures = new VkPhysicalDeviceFeatures();

            VkDeviceCreateInfo deviceCreateInfo = VkDeviceCreateInfo.New();

            fixed (VkDeviceQueueCreateInfo* qciPtr = &queueCreateInfos.Items[0])
            {
                deviceCreateInfo.pQueueCreateInfos = qciPtr;
                deviceCreateInfo.queueCreateInfoCount = queueCreateInfos.Count;

                deviceCreateInfo.pEnabledFeatures = &deviceFeatures;

                byte* layerNames = Strings.StandardValidationLayerName;
                deviceCreateInfo.enabledLayerCount = 1;
                deviceCreateInfo.ppEnabledLayerNames = &layerNames;

                byte* extensionNames = Strings.VK_KHR_SWAPCHAIN_EXTENSION_NAME;
                deviceCreateInfo.enabledExtensionCount = 1;
                deviceCreateInfo.ppEnabledExtensionNames = &extensionNames;

                vkCreateDevice(_physicalDevice, ref deviceCreateInfo, null, out _device);
            }

            vkGetDeviceQueue(_device, _graphicsQueueIndex, 0, out _graphicsQueue);
            vkGetDeviceQueue(_device, _presentQueueIndex, 0, out _presentQueue);
        }

        private void GetQueueFamilyIndices()
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

                vkGetPhysicalDeviceSurfaceSupportKHR(_physicalDevice, i, _surface, out VkBool32 presentSupported);
                if (presentSupported)
                {
                    _presentQueueIndex = i;
                }
            }
        }

        private void CreatePlatformWindow()
        {
            _window = new Sdl2Window("Triangle From Scratch", 100, 100, 1280, 760, SDL_WindowFlags.Resizable, false);
            _window.Visible = true;
        }

        private void CreateSurface()
        {
            SDL_SysWMinfo sysWmInfo;
            SDL_GetVersion(&sysWmInfo.version);
            SDL_GetWMWindowInfo(_window.SdlWindowHandle, &sysWmInfo);

            switch (sysWmInfo.subsystem)
            {
                case SysWMType.Windows:
                    Win32WindowInfo win32Info = Unsafe.Read<Win32WindowInfo>(&sysWmInfo.info);
                    VkWin32SurfaceCreateInfoKHR wsci = VkWin32SurfaceCreateInfoKHR.New();
                    wsci.hinstance = win32Info.hinstance;
                    wsci.hwnd = win32Info.window;
                    vkCreateWin32SurfaceKHR(_instance, ref wsci, null, out _surface);
                    break;
                case SysWMType.X11:
                    X11WindowInfo x11Info = Unsafe.Read<X11WindowInfo>(&sysWmInfo.info);
                    VkXlibSurfaceCreateInfoKHR xsci = VkXlibSurfaceCreateInfoKHR.New();
                    xsci.dpy = (Vulkan.Xlib.Display*)x11Info.display;
                    xsci.window = new Vulkan.Xlib.Window { Value = x11Info.window };
                    vkCreateXlibSurfaceKHR(_instance, ref xsci, null, out _surface);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        private void CreateSwapchain()
        {
            uint surfaceFormatCount = 0;
            vkGetPhysicalDeviceSurfaceFormatsKHR(_physicalDevice, _surface, ref surfaceFormatCount, null);
            VkSurfaceFormatKHR[] formats = new VkSurfaceFormatKHR[surfaceFormatCount];
            vkGetPhysicalDeviceSurfaceFormatsKHR(_physicalDevice, _surface, ref surfaceFormatCount, out formats[0]);

            VkSurfaceFormatKHR surfaceFormat = new VkSurfaceFormatKHR();
            if (formats.Length == 1 && formats[0].format == VkFormat.Undefined)
            {
                surfaceFormat = new VkSurfaceFormatKHR { colorSpace = VkColorSpaceKHR.SrgbNonlinear, format = VkFormat.B8g8r8a8Unorm };
            }
            else
            {
                foreach (VkSurfaceFormatKHR format in formats)
                {
                    if (format.colorSpace == VkColorSpaceKHR.SrgbNonlinear && format.format == VkFormat.B8g8r8a8Unorm)
                    {
                        surfaceFormat = format;
                        break;
                    }
                }
                if (surfaceFormat.format == VkFormat.Undefined)
                {
                    surfaceFormat = formats[0];
                }
            }

            uint presentModeCount = 0;
            vkGetPhysicalDeviceSurfacePresentModesKHR(_physicalDevice, _surface, ref presentModeCount, null);
            VkPresentModeKHR[] presentModes = new VkPresentModeKHR[presentModeCount];
            vkGetPhysicalDeviceSurfacePresentModesKHR(_physicalDevice, _surface, ref presentModeCount, out presentModes[0]);

            VkPresentModeKHR presentMode = VkPresentModeKHR.Fifo;
            if (presentModes.Contains(VkPresentModeKHR.Mailbox))
            {
                presentMode = VkPresentModeKHR.Mailbox;
            }
            else if (presentModes.Contains(VkPresentModeKHR.Immediate))
            {
                presentMode = VkPresentModeKHR.Immediate;
            }

            vkGetPhysicalDeviceSurfaceCapabilitiesKHR(_physicalDevice, _surface, out VkSurfaceCapabilitiesKHR surfaceCapabilities);
            uint imageCount = surfaceCapabilities.minImageCount + 1;

            VkSwapchainCreateInfoKHR sci = VkSwapchainCreateInfoKHR.New();
            sci.surface = _surface;
            sci.presentMode = presentMode;
            sci.imageFormat = surfaceFormat.format;
            sci.imageColorSpace = surfaceFormat.colorSpace;
            sci.imageExtent = new VkExtent2D { width = (uint)_window.Width, height = (uint)_window.Height };
            sci.minImageCount = imageCount;
            sci.imageArrayLayers = 1;
            sci.imageUsage = VkImageUsageFlags.ColorAttachment;

            FixedArray2<uint> queueFamilyIndices = new FixedArray2<uint>(_graphicsQueueIndex, _presentQueueIndex);

            if (_graphicsQueueIndex != _presentQueueIndex)
            {
                sci.imageSharingMode = VkSharingMode.Concurrent;
                sci.queueFamilyIndexCount = 2;
                sci.pQueueFamilyIndices = &queueFamilyIndices.First;
            }
            else
            {
                sci.imageSharingMode = VkSharingMode.Exclusive;
                sci.queueFamilyIndexCount = 0;
            }

            sci.preTransform = surfaceCapabilities.currentTransform;
            sci.compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque;
            sci.clipped = true;

            vkCreateSwapchainKHR(_device, ref sci, null, out _swapchain);

            // Get the images
            uint scImageCount = 0;
            vkGetSwapchainImagesKHR(_device, _swapchain, ref scImageCount, null);
            _scImages.Count = scImageCount;
            vkGetSwapchainImagesKHR(_device, _swapchain, ref scImageCount, out _scImages.Items[0]);

            _swapchainImageFormat = surfaceFormat.format;
            _swapchainExtent = sci.imageExtent;
        }

        private void CreateImageViews()
        {
            _scImageViews.Resize(_scImages.Count);
            for (int i = 0; i < _scImages.Count; i++)
            {
                VkImageViewCreateInfo ivci = VkImageViewCreateInfo.New();
                ivci.viewType = VkImageViewType._2d;
                ivci.format = _swapchainImageFormat;
                ivci.subresourceRange.aspectMask = VkImageAspectFlags.Color;
                ivci.subresourceRange.baseMipLevel = 0;
                ivci.subresourceRange.levelCount = 1;
                ivci.subresourceRange.baseArrayLayer = 0;
                ivci.subresourceRange.layerCount = 1;
                vkCreateImageView(_device, ref ivci, null, out VkImageView imageView);
                _scImageViews[i] = imageView;
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