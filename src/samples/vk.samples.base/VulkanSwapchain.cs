// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems: https://github.com/SaschaWillems/Vulkan
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: base/VulkanSwapChain.hpp, 

/*
* Vulkan Example base class
*
* Copyright (C) 2016 by Sascha Willems - www.saschawillems.de
*
* This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Vulkan;
using Veldrid.Sdl2;
using static Vulkan.VulkanNative;
using static Veldrid.Sdl2.Sdl2Native;

namespace Vk.Samples
{
    public unsafe class VulkanSwapchain
    {
        public VkInstance Instance { get; private set; }
        public VkPhysicalDevice PhysicalDevice { get; private set; }
        public VkDevice Device { get; private set; }
        public VkSurfaceKHR Surface { get; private set; }
        public uint QueueNodeIndex { get; private set; } = uint.MaxValue;
        public VkFormat ColorFormat { get; private set; }
        public VkColorSpaceKHR ColorSpace { get; private set; }
        public VkSwapchainKHR Swapchain { get; private set; }
        public uint ImageCount { get; private set; }
        public NativeList<VkImage> Images { get; set; } = new NativeList<VkImage>();
        public NativeList<SwapChainBuffer> Buffers { get; set; } = new NativeList<SwapChainBuffer>();

        public void Connect(VkInstance instance, VkPhysicalDevice physicalDevice, VkDevice device)
        {
            Instance = instance;
            PhysicalDevice = physicalDevice;
            Device = device;
        }

        public unsafe void InitSurface(IntPtr sdlWindow)
        {
            SDL_version version;
            SDL_GetVersion(&version);
            SDL_SysWMinfo sysWmInfo;
            sysWmInfo.version = version;
            int result = SDL_GetWMWindowInfo(sdlWindow, &sysWmInfo);
            if (result == 0)
            {
                throw new InvalidOperationException("Couldn't retrieve SDL window info.");
            }
            VkResult err;
            if (sysWmInfo.subsystem == SysWMType.Windows)
            {
                Win32WindowInfo win32Info = Unsafe.Read<Win32WindowInfo>(&sysWmInfo.info);
                // Create the os-specific Surface
                VkWin32SurfaceCreateInfoKHR surfaceCreateInfo = VkWin32SurfaceCreateInfoKHR.New();
                var processHandle = Process.GetCurrentProcess().SafeHandle.DangerousGetHandle();
                surfaceCreateInfo.hinstance = processHandle;
                surfaceCreateInfo.hwnd = win32Info.Sdl2Window;
                VkSurfaceKHR surface;
                err = vkCreateWin32SurfaceKHR(Instance, &surfaceCreateInfo, null, &surface);
                Surface = surface;
            }
            else if (sysWmInfo.subsystem == SysWMType.X11)
            {
                X11WindowInfo x11Info = Unsafe.Read<X11WindowInfo>(&sysWmInfo.info);
                VkXlibSurfaceCreateInfoKHR surfaceCreateInfo = VkXlibSurfaceCreateInfoKHR.New();
                surfaceCreateInfo.dpy = (Vulkan.Xlib.Display*)x11Info.display;
                surfaceCreateInfo.window = new Vulkan.Xlib.Window { Value = x11Info.Sdl2Window };
                VkSurfaceKHR surface;
                err = vkCreateXlibSurfaceKHR(Instance, &surfaceCreateInfo, null, out surface);
                Surface = surface;
            }
            else
            {
                throw new NotImplementedException($"SDL backend not implemented: {sysWmInfo.subsystem}.");
            }

            // Get available queue family properties
            uint queueCount;
            vkGetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &queueCount, null);
            Debug.Assert(queueCount >= 1);

            using (NativeList<VkQueueFamilyProperties> queueProps = new NativeList<VkQueueFamilyProperties>(queueCount))
            {
                vkGetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &queueCount, (VkQueueFamilyProperties*)queueProps.Data.ToPointer());
                queueProps.Count = queueCount;

                // Iterate over each queue to learn whether it supports presenting:
                // Find a queue with present support
                // Will be used to present the swap chain Images to the windowing system
                VkBool32* supportsPresent = stackalloc VkBool32[(int)queueCount];

                for (uint i = 0; i < queueCount; i++)
                {
                    vkGetPhysicalDeviceSurfaceSupportKHR(PhysicalDevice, i, Surface, &supportsPresent[i]);
                }

                // Search for a graphics and a present queue in the array of queue
                // families, try to find one that supports both
                uint graphicsQueueNodeIndex = uint.MaxValue;
                uint presentQueueNodeIndex = uint.MaxValue;
                for (uint i = 0; i < queueCount; i++)
                {
                    if ((queueProps[i].queueFlags & VkQueueFlags.Graphics) != 0)
                    {
                        if (graphicsQueueNodeIndex == uint.MaxValue)
                        {
                            graphicsQueueNodeIndex = i;
                        }

                        if (supportsPresent[i] == True)
                        {
                            graphicsQueueNodeIndex = i;
                            presentQueueNodeIndex = i;
                            break;
                        }
                    }
                }

                if (presentQueueNodeIndex == uint.MaxValue)
                {
                    // If there's no queue that supports both present and graphics
                    // try to find a separate present queue
                    for (uint i = 0; i < queueCount; ++i)
                    {
                        if (supportsPresent[i] == True)
                        {
                            presentQueueNodeIndex = i;
                            break;
                        }
                    }
                }

                // Exit if either a graphics or a presenting queue hasn't been found
                if (graphicsQueueNodeIndex == uint.MaxValue || presentQueueNodeIndex == uint.MaxValue)
                {
                    throw new InvalidOperationException("Could not find a graphics and/or presenting queue!");
                }

                // todo : Add support for separate graphics and presenting queue
                if (graphicsQueueNodeIndex != presentQueueNodeIndex)
                {
                    throw new InvalidOperationException("Separate graphics and presenting queues are not supported yet!");
                }

                QueueNodeIndex = graphicsQueueNodeIndex;

                // Get list of supported Surface formats
                uint formatCount;
                err = vkGetPhysicalDeviceSurfaceFormatsKHR(PhysicalDevice, Surface, &formatCount, null);
                Debug.Assert(err == VkResult.Success);
                Debug.Assert(formatCount > 0);

                using (NativeList<VkSurfaceFormatKHR> surfaceFormats = new NativeList<VkSurfaceFormatKHR>(formatCount))
                {
                    err = vkGetPhysicalDeviceSurfaceFormatsKHR(PhysicalDevice, Surface, &formatCount, (VkSurfaceFormatKHR*)surfaceFormats.Data.ToPointer());
                    surfaceFormats.Count = formatCount;
                    Debug.Assert(err == VkResult.Success);

                    // If the Surface format list only includes one entry with VK_FORMAT_UNDEFINED,
                    // there is no preferered format, so we assume VK_FORMAT_B8G8R8A8_UNORM
                    if ((formatCount == 1) && (surfaceFormats[0].format == VkFormat.Undefined))
                    {
                        ColorFormat = VkFormat.B8g8r8a8Unorm;
                        ColorSpace = surfaceFormats[0].colorSpace;
                    }
                    else
                    {
                        // iterate over the list of available Surface format and
                        // check for the presence of VK_FORMAT_B8G8R8A8_UNORM
                        bool found_B8G8R8A8_UNORM = false;
                        foreach (var surfaceFormat in surfaceFormats)
                        {
                            if (surfaceFormat.format == VkFormat.B8g8r8a8Unorm)
                            {
                                ColorFormat = surfaceFormat.format;
                                ColorSpace = surfaceFormat.colorSpace;
                                found_B8G8R8A8_UNORM = true;
                                break;
                            }
                        }

                        // in case VK_FORMAT_B8G8R8A8_UNORM is not available
                        // select the first available color format
                        if (!found_B8G8R8A8_UNORM)
                        {
                            ColorFormat = surfaceFormats[0].format;
                            ColorSpace = surfaceFormats[0].colorSpace;
                        }
                    }
                }
            }
        }

        /** 
        * Create the swapchain and get it's Images with given width and height
        * 
        * @param width Pointer to the width of the swapchain (may be adjusted to fit the requirements of the swapchain)
        * @param height Pointer to the height of the swapchain (may be adjusted to fit the requirements of the swapchain)
        * @param vsync (Optional) Can be used to force vsync'd rendering (by using VK_PRESENT_MODE_FIFO_KHR as presentation mode)
        */
        public void Create(uint* width, uint* height, bool vsync = false)
        {
            VkResult err;
            VkSwapchainKHR oldSwapchain = Swapchain;

            // Get physical Device Surface properties and formats
            VkSurfaceCapabilitiesKHR surfCaps;
            err = vkGetPhysicalDeviceSurfaceCapabilitiesKHR(PhysicalDevice, Surface, &surfCaps);
            Debug.Assert(err == VkResult.Success);

            // Get available present modes
            uint presentModeCount;
            err = vkGetPhysicalDeviceSurfacePresentModesKHR(PhysicalDevice, Surface, &presentModeCount, null);
            Debug.Assert(err == VkResult.Success);
            Debug.Assert(presentModeCount > 0);

            using (NativeList<VkPresentModeKHR> presentModes = new NativeList<VkPresentModeKHR>(presentModeCount))
            {
                err = vkGetPhysicalDeviceSurfacePresentModesKHR(PhysicalDevice, Surface, &presentModeCount, (VkPresentModeKHR*)presentModes.Data);
                Debug.Assert(err == VkResult.Success);
                presentModes.Count = presentModeCount;

                VkExtent2D swapchainExtent;
                // If width (and height) equals the special value 0xFFFFFFFF, the size of the Surface will be set by the swapchain
                if (surfCaps.currentExtent.width == unchecked((uint)-1))
                {
                    // If the Surface size is undefined, the size is set to
                    // the size of the Images requested.
                    swapchainExtent.width = *width;
                    swapchainExtent.height = *height;
                }
                else
                {
                    // If the Surface size is defined, the swap chain size must match
                    swapchainExtent = surfCaps.currentExtent;
                    *width = surfCaps.currentExtent.width;
                    *height = surfCaps.currentExtent.height;
                }


                // Select a present mode for the swapchain

                // The VK_PRESENT_MODE_FIFO_KHR mode must always be present as per spec
                // This mode waits for the vertical blank ("v-sync")
                VkPresentModeKHR swapchainPresentMode = VkPresentModeKHR.FifoKHR;

                // If v-sync is not requested, try to find a mailbox mode
                // It's the lowest latency non-tearing present mode available
                if (!vsync)
                {
                    for (uint i = 0; i < presentModeCount; i++)
                    {
                        if (presentModes[i] == VkPresentModeKHR.MailboxKHR)
                        {
                            swapchainPresentMode = VkPresentModeKHR.MailboxKHR;
                            break;
                        }
                        if ((swapchainPresentMode != VkPresentModeKHR.MailboxKHR) && (presentModes[i] == VkPresentModeKHR.ImmediateKHR))
                        {
                            swapchainPresentMode = VkPresentModeKHR.ImmediateKHR;
                        }
                    }
                }

                // Determine the number of Images
                uint desiredNumberOfSwapchainImages = surfCaps.minImageCount + 1;
                if ((surfCaps.maxImageCount > 0) && (desiredNumberOfSwapchainImages > surfCaps.maxImageCount))
                {
                    desiredNumberOfSwapchainImages = surfCaps.maxImageCount;
                }

                // Find the transformation of the Surface
                VkSurfaceTransformFlagsKHR preTransform;
                if ((surfCaps.supportedTransforms & VkSurfaceTransformFlagsKHR.IdentityKHR) != 0)
                {
                    // We prefer a non-rotated transform
                    preTransform = VkSurfaceTransformFlagsKHR.IdentityKHR;
                }
                else
                {
                    preTransform = surfCaps.currentTransform;
                }

                VkSwapchainCreateInfoKHR swapchainCI = VkSwapchainCreateInfoKHR.New();
                swapchainCI.pNext = null;
                swapchainCI.surface = Surface;
                swapchainCI.minImageCount = desiredNumberOfSwapchainImages;
                swapchainCI.imageFormat = ColorFormat;
                swapchainCI.imageColorSpace = ColorSpace;
                swapchainCI.imageExtent = new VkExtent2D() { width = swapchainExtent.width, height = swapchainExtent.height };
                swapchainCI.imageUsage = VkImageUsageFlags.ColorAttachment;
                swapchainCI.preTransform = preTransform;
                swapchainCI.imageArrayLayers = 1;
                swapchainCI.imageSharingMode = VkSharingMode.Exclusive;
                swapchainCI.queueFamilyIndexCount = 0;
                swapchainCI.pQueueFamilyIndices = null;
                swapchainCI.presentMode = swapchainPresentMode;
                swapchainCI.oldSwapchain = oldSwapchain;
                // Setting clipped to VK_TRUE allows the implementation to discard rendering outside of the Surface area
                swapchainCI.clipped = True;
                swapchainCI.compositeAlpha = VkCompositeAlphaFlagsKHR.OpaqueKHR;

                // Set additional usage flag for blitting from the swapchain Images if supported
                VkFormatProperties formatProps;
                vkGetPhysicalDeviceFormatProperties(PhysicalDevice, ColorFormat, out formatProps);
                if ((formatProps.optimalTilingFeatures & VkFormatFeatureFlags.BlitDst) != 0)
                {
                    swapchainCI.imageUsage |= VkImageUsageFlags.TransferSrc;
                }

                VkSwapchainKHR swapChain;
                err = vkCreateSwapchainKHR(Device, &swapchainCI, null, out swapChain);
                Debug.Assert(err == VkResult.Success);
                Swapchain = swapChain;

                // If an existing swap chain is re-created, destroy the old swap chain
                // This also cleans up all the presentable Images
                if (oldSwapchain.Handle != 0)
                {
                    for (uint i = 0; i < ImageCount; i++)
                    {
                        vkDestroyImageView(Device, Buffers[i].View, null);
                    }
                    vkDestroySwapchainKHR(Device, oldSwapchain, null);
                }

                uint imageCount;
                err = vkGetSwapchainImagesKHR(Device, swapChain, &imageCount, null);
                Debug.Assert(err == VkResult.Success);
                ImageCount = imageCount;

                // Get the swap chain Images
                Images.Resize(imageCount);
                err = vkGetSwapchainImagesKHR(Device, swapChain, &imageCount, (VkImage*)Images.Data.ToPointer());
                Images.Count = imageCount;
                Debug.Assert(err == VkResult.Success);

                // Get the swap chain Buffers containing the image and imageview
                Buffers.Resize(imageCount);
                Buffers.Count = imageCount;
                for (uint i = 0; i < imageCount; i++)
                {
                    VkImageViewCreateInfo colorAttachmentView = new VkImageViewCreateInfo();
                    colorAttachmentView.sType = VkStructureType.ImageViewCreateInfo;
                    colorAttachmentView.pNext = null;
                    colorAttachmentView.format = ColorFormat;
                    colorAttachmentView.components = new VkComponentMapping()
                    {
                        r = VkComponentSwizzle.R,
                        g = VkComponentSwizzle.G,
                        b = VkComponentSwizzle.B,
                        a = VkComponentSwizzle.A
                    };

                    colorAttachmentView.subresourceRange.aspectMask = VkImageAspectFlags.Color;
                    colorAttachmentView.subresourceRange.baseMipLevel = 0;
                    colorAttachmentView.subresourceRange.levelCount = 1;
                    colorAttachmentView.subresourceRange.baseArrayLayer = 0;
                    colorAttachmentView.subresourceRange.layerCount = 1;
                    colorAttachmentView.viewType = VkImageViewType.Image2D;
                    colorAttachmentView.flags = 0;

                    Buffers[i].Image = Images[i];

                    colorAttachmentView.image = Buffers[i].Image;

                    VkImageView view;
                    err = vkCreateImageView(Device, &colorAttachmentView, null, &view);
                    Buffers[i].View = view;
                    Debug.Assert(err == VkResult.Success);
                }
            }
        }

        /** 
        * Acquires the next image in the swap chain
        *
        * @param presentCompleteSemaphore (Optional) Semaphore that is signaled when the image is ready for use
        * @param imageIndex Pointer to the image index that will be increased if the next image could be acquired
        *
        * @note The function will always wait until the next image has been acquired by setting timeout to UINT64_MAX
        *
        * @return VkResult of the image acquisition
        */
        public VkResult AcquireNextImage(VkSemaphore presentCompleteSemaphore, ref uint imageIndex)
        {
            // By setting timeout to UINT64_MAX we will always wait until the next image has been acquired or an actual error is thrown
            // With that we don't have to handle VK_NOT_READY
            return vkAcquireNextImageKHR(Device, Swapchain, ulong.MaxValue, presentCompleteSemaphore, new VkFence(), ref imageIndex);
        }

        /**
        * Queue an image for presentation
        *
        * @param queue Presentation queue for presenting the image
        * @param imageIndex Index of the swapchain image to queue for presentation
        * @param waitSemaphore (Optional) Semaphore that is waited on before the image is presented (only used if != VK_NULL_HANDLE)
        *
        * @return VkResult of the queue presentation
        */
        public VkResult QueuePresent(VkQueue queue, uint imageIndex, VkSemaphore waitSemaphore = new VkSemaphore())
        {
            VkPresentInfoKHR presentInfo = VkPresentInfoKHR.New();
            presentInfo.pNext = null;
            presentInfo.swapchainCount = 1;
            var sc = Swapchain;
            presentInfo.pSwapchains = &sc;
            presentInfo.pImageIndices = &imageIndex;
            // Check if a wait semaphore has been specified to wait for before presenting the image
            if (waitSemaphore.Handle != 0)
            {
                presentInfo.pWaitSemaphores = &waitSemaphore;
                presentInfo.waitSemaphoreCount = 1;
            }
            return vkQueuePresentKHR(queue, &presentInfo);
        }
    }

    public struct SwapChainBuffer
    {
        public VkImage Image;
        public VkImageView View;
    }
}
