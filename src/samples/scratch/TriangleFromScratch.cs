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
using System.IO;

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
        private VkPipelineLayout _pipelineLayout;
        private VkRenderPass _renderPass;
        private VkPipeline _graphicsPipeline;
        private VkCommandPool _commandPool;
        private RawList<VkCommandBuffer> _commandBuffers = new RawList<VkCommandBuffer>();
        private VkSemaphore _imageAvailableSemaphore;
        private VkSemaphore _renderCompleteSemaphore;

        // Swapchain stuff
        private RawList<VkImage> _scImages = new RawList<VkImage>();
        private RawList<VkImageView> _scImageViews = new RawList<VkImageView>();
        private RawList<VkFramebuffer> _scFramebuffers = new RawList<VkFramebuffer>();
        private VkSwapchainKHR _swapchain;
        private VkFormat _scImageFormat;
        private VkExtent2D _scExtent;

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
            CreateRenderPass();
            CreateGraphicsPipeline();
            CreateFramebuffers();
            CreateCommandPool();
            CreateCommandBuffers();
            CreateSemaphores();

            RunMainLoop();
        }

        private void RunMainLoop()
        {
            while (_window.Exists)
            {
                _window.GetInputSnapshot();
                DrawFrame();
            }
        }

        private void DrawFrame()
        {
            uint imageIndex = 0;
            vkAcquireNextImageKHR(_device, _swapchain, ulong.MaxValue, _imageAvailableSemaphore, VkFence.Null, ref imageIndex);

            VkSubmitInfo submitInfo = VkSubmitInfo.New();
            VkSemaphore waitSemaphore = _imageAvailableSemaphore;
            VkPipelineStageFlags waitStages = VkPipelineStageFlags.ColorAttachmentOutput;
            submitInfo.waitSemaphoreCount = 1;
            submitInfo.pWaitSemaphores = &waitSemaphore;
            submitInfo.pWaitDstStageMask = &waitStages;
            VkCommandBuffer cb = _commandBuffers[imageIndex];
            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = &cb;
            VkSemaphore signalSemaphore = _renderCompleteSemaphore;
            submitInfo.signalSemaphoreCount = 1;
            submitInfo.pSignalSemaphores = &signalSemaphore;
            vkQueueSubmit(_graphicsQueue, 1, ref submitInfo, VkFence.Null);

            VkPresentInfoKHR presentInfo = VkPresentInfoKHR.New();
            presentInfo.waitSemaphoreCount = 1;
            presentInfo.pWaitSemaphores = &signalSemaphore;

            VkSwapchainKHR swapchain = _swapchain;
            presentInfo.swapchainCount = 1;
            presentInfo.pSwapchains = &swapchain;
            presentInfo.pImageIndices = &imageIndex;

            vkQueuePresentKHR(_presentQueue, ref presentInfo);
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

            bool debug = false;
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
            VkQueue q;
            vkGetDeviceQueue(_device, _presentQueueIndex, 0, out q);
            _presentQueue = q;
        }

        private void GetQueueFamilyIndices()
        {
            uint queueFamilyCount = 0;
            vkGetPhysicalDeviceQueueFamilyProperties(_physicalDevice, ref queueFamilyCount, null);
            VkQueueFamilyProperties[] qfp = new VkQueueFamilyProperties[queueFamilyCount];
            vkGetPhysicalDeviceQueueFamilyProperties(_physicalDevice, ref queueFamilyCount, out qfp[0]);

            bool foundGraphics = false;
            bool foundPresent = false;

            for (uint i = 0; i < qfp.Length; i++)
            {
                if ((qfp[i].queueFlags & VkQueueFlags.Graphics) != 0)
                {
                    _graphicsQueueIndex = i;
                    foundGraphics = true;
                }

                vkGetPhysicalDeviceSurfaceSupportKHR(_physicalDevice, i, _surface, out VkBool32 presentSupported);
                if (presentSupported)
                {
                    _presentQueueIndex = i;
                    foundPresent = true;
                }

                if (foundGraphics && foundPresent)
                {
                    break;
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

            _scImageFormat = surfaceFormat.format;
            _scExtent = sci.imageExtent;
        }

        private void CreateImageViews()
        {
            _scImageViews.Resize(_scImages.Count);
            for (int i = 0; i < _scImages.Count; i++)
            {
                VkImageViewCreateInfo ivci = VkImageViewCreateInfo.New();
                ivci.viewType = VkImageViewType._2d;
                ivci.format = _scImageFormat;
                ivci.subresourceRange.aspectMask = VkImageAspectFlags.Color;
                ivci.subresourceRange.baseMipLevel = 0;
                ivci.subresourceRange.levelCount = 1;
                ivci.subresourceRange.baseArrayLayer = 0;
                ivci.subresourceRange.layerCount = 1;
                ivci.image = _scImages[i];
                VkImageView imageView;
                vkCreateImageView(_device, ref ivci, null, &imageView);
                _scImageViews[i] = imageView;
            }
        }

        private void CreateRenderPass()
        {
            VkAttachmentDescription colorAttachment = new VkAttachmentDescription();
            colorAttachment.format = _scImageFormat;
            colorAttachment.samples = VkSampleCountFlags._1;
            colorAttachment.loadOp = VkAttachmentLoadOp.Clear;
            colorAttachment.storeOp = VkAttachmentStoreOp.Store;
            colorAttachment.stencilLoadOp = VkAttachmentLoadOp.DontCare;
            colorAttachment.stencilStoreOp = VkAttachmentStoreOp.DontCare;
            colorAttachment.initialLayout = VkImageLayout.Undefined;
            colorAttachment.finalLayout = VkImageLayout.PresentSrc;

            VkAttachmentReference colorAttachmentRef = new VkAttachmentReference();
            colorAttachmentRef.attachment = 0;
            colorAttachmentRef.layout = VkImageLayout.ColorAttachmentOptimal;

            VkSubpassDescription subpass = new VkSubpassDescription();
            subpass.pipelineBindPoint = VkPipelineBindPoint.Graphics;
            subpass.colorAttachmentCount = 1;
            subpass.pColorAttachments = &colorAttachmentRef;

            VkRenderPassCreateInfo renderPassCI = VkRenderPassCreateInfo.New();
            renderPassCI.attachmentCount = 1;
            renderPassCI.pAttachments = &colorAttachment;
            renderPassCI.subpassCount = 1;
            renderPassCI.pSubpasses = &subpass;

            VkSubpassDependency dependency = new VkSubpassDependency();
            dependency.srcSubpass = SubpassExternal;
            dependency.dstSubpass = 0;
            dependency.srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            dependency.srcAccessMask = 0;
            dependency.dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            dependency.dstAccessMask = VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite;

            renderPassCI.dependencyCount = 1;
            renderPassCI.pDependencies = &dependency;

            vkCreateRenderPass(_device, ref renderPassCI, null, out _renderPass);
        }

        private void CreateGraphicsPipeline()
        {
            byte[] vertBytes = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Shaders", "shader.vert.spv"));
            byte[] fragBytes = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Shaders", "shader.frag.spv"));

            VkShaderModule vertexShader = CreateShader(vertBytes);
            VkShaderModule fragmentShader = CreateShader(fragBytes);

            VkPipelineShaderStageCreateInfo vertCreateInfo = VkPipelineShaderStageCreateInfo.New();
            vertCreateInfo.stage = VkShaderStageFlags.Vertex;
            vertCreateInfo.module = vertexShader;
            vertCreateInfo.pName = Strings.main;

            VkPipelineShaderStageCreateInfo fragCreateInfo = VkPipelineShaderStageCreateInfo.New();
            fragCreateInfo.stage = VkShaderStageFlags.Fragment;
            fragCreateInfo.module = fragmentShader;
            fragCreateInfo.pName = Strings.main;

            FixedArray2<VkPipelineShaderStageCreateInfo> shaderStageCreateInfos
                = new FixedArray2<VkPipelineShaderStageCreateInfo>(vertCreateInfo, fragCreateInfo);

            VkPipelineVertexInputStateCreateInfo vertexInputStateCreateInfo = VkPipelineVertexInputStateCreateInfo.New();
            // Nothing: default values for everything.

            VkPipelineInputAssemblyStateCreateInfo inputAssemblyCI = VkPipelineInputAssemblyStateCreateInfo.New();
            inputAssemblyCI.primitiveRestartEnable = false;
            inputAssemblyCI.topology = VkPrimitiveTopology.TriangleList;

            VkViewport viewport = new VkViewport();
            viewport.x = 0;
            viewport.y = 0;
            viewport.width = _scExtent.width;
            viewport.height = _scExtent.height;
            viewport.minDepth = 0f;
            viewport.maxDepth = 1f;

            VkRect2D scissorRect = new VkRect2D() { extent = _scExtent };

            VkPipelineViewportStateCreateInfo viewportStateCI = VkPipelineViewportStateCreateInfo.New();
            viewportStateCI.viewportCount = 1;
            viewportStateCI.pViewports = &viewport;
            viewportStateCI.scissorCount = 1;
            viewportStateCI.pScissors = &scissorRect;

            VkPipelineRasterizationStateCreateInfo rasterizerStateCI = VkPipelineRasterizationStateCreateInfo.New();
            rasterizerStateCI.cullMode = VkCullModeFlags.Back;
            rasterizerStateCI.polygonMode = VkPolygonMode.Fill;
            rasterizerStateCI.lineWidth = 1f;
            rasterizerStateCI.frontFace = VkFrontFace.Clockwise;

            VkPipelineMultisampleStateCreateInfo multisampleStateCI = VkPipelineMultisampleStateCreateInfo.New();
            multisampleStateCI.rasterizationSamples = VkSampleCountFlags._1;
            multisampleStateCI.minSampleShading = 1f;

            VkPipelineColorBlendAttachmentState colorBlendAttachementState = new VkPipelineColorBlendAttachmentState();
            colorBlendAttachementState.colorWriteMask = VkColorComponentFlags.R | VkColorComponentFlags.G | VkColorComponentFlags.B | VkColorComponentFlags.A;
            colorBlendAttachementState.blendEnable = false;

            VkPipelineColorBlendStateCreateInfo colorBlendStateCI = VkPipelineColorBlendStateCreateInfo.New();
            colorBlendStateCI.attachmentCount = 1;
            colorBlendStateCI.pAttachments = &colorBlendAttachementState;

            VkPipelineLayoutCreateInfo pipelineLayoutCI = VkPipelineLayoutCreateInfo.New();
            vkCreatePipelineLayout(_device, ref pipelineLayoutCI, null, out _pipelineLayout);

            VkGraphicsPipelineCreateInfo graphicsPipelineCI = VkGraphicsPipelineCreateInfo.New();
            graphicsPipelineCI.stageCount = shaderStageCreateInfos.Count;
            graphicsPipelineCI.pStages = &shaderStageCreateInfos.First;

            graphicsPipelineCI.pVertexInputState = &vertexInputStateCreateInfo;
            graphicsPipelineCI.pInputAssemblyState = &inputAssemblyCI;
            graphicsPipelineCI.pViewportState = &viewportStateCI;
            graphicsPipelineCI.pRasterizationState = &rasterizerStateCI;
            graphicsPipelineCI.pMultisampleState = &multisampleStateCI;
            graphicsPipelineCI.pColorBlendState = &colorBlendStateCI;

            graphicsPipelineCI.layout = _pipelineLayout;

            graphicsPipelineCI.renderPass = _renderPass;
            graphicsPipelineCI.subpass = 0;

            vkCreateGraphicsPipelines(_device, VkPipelineCache.Null, 1, ref graphicsPipelineCI, null, out _graphicsPipeline);
        }

        private void CreateFramebuffers()
        {
            _scFramebuffers.Resize(_scImageViews.Count);
            for (uint i = 0; i < _scImageViews.Count; i++)
            {
                VkImageView attachment = _scImageViews[i];
                VkFramebufferCreateInfo framebufferCI = VkFramebufferCreateInfo.New();
                framebufferCI.renderPass = _renderPass;
                framebufferCI.attachmentCount = 1;
                framebufferCI.pAttachments = &attachment;
                framebufferCI.width = _scExtent.width;
                framebufferCI.height = _scExtent.height;
                framebufferCI.layers = 1;

                vkCreateFramebuffer(_device, ref framebufferCI, null, out _scFramebuffers[i]);
            }
        }

        private void CreateCommandPool()
        {
            VkCommandPoolCreateInfo commandPoolCI = VkCommandPoolCreateInfo.New();
            commandPoolCI.queueFamilyIndex = _graphicsQueueIndex;
            vkCreateCommandPool(_device, ref commandPoolCI, null, out _commandPool);
        }

        private void CreateCommandBuffers()
        {
            _commandBuffers.Resize(_scFramebuffers.Count);
            VkCommandBufferAllocateInfo commandBufferAI = VkCommandBufferAllocateInfo.New();
            commandBufferAI.commandPool = _commandPool;
            commandBufferAI.level = VkCommandBufferLevel.Primary;
            commandBufferAI.commandBufferCount = _commandBuffers.Count;
            vkAllocateCommandBuffers(_device, ref commandBufferAI, out _commandBuffers[0]);

            for (uint i = 0; i < _commandBuffers.Count; i++)
            {
                VkCommandBuffer cb = _commandBuffers[i];

                VkCommandBufferBeginInfo beginInfo = VkCommandBufferBeginInfo.New();
                beginInfo.flags = VkCommandBufferUsageFlags.SimultaneousUse;
                vkBeginCommandBuffer(cb, ref beginInfo);

                VkRenderPassBeginInfo rpbi = VkRenderPassBeginInfo.New();
                rpbi.renderPass = _renderPass;
                rpbi.framebuffer = _scFramebuffers[i];
                rpbi.renderArea.extent = _scExtent;

                VkClearValue clearValue = new VkClearValue() { color = new VkClearColorValue() };
                rpbi.clearValueCount = 1;
                rpbi.pClearValues = &clearValue;

                vkCmdBeginRenderPass(cb, ref rpbi, VkSubpassContents.Inline);

                vkCmdBindPipeline(cb, VkPipelineBindPoint.Graphics, _graphicsPipeline);
                vkCmdDraw(cb, 3, 1, 0, 0);

                vkCmdEndRenderPass(cb);

                vkEndCommandBuffer(cb);
            }
        }

        private void CreateSemaphores()
        {
            VkSemaphoreCreateInfo semaphoreCI = VkSemaphoreCreateInfo.New();
            vkCreateSemaphore(_device, ref semaphoreCI, null, out _imageAvailableSemaphore);
            vkCreateSemaphore(_device, ref semaphoreCI, null, out _renderCompleteSemaphore);
        }

        private VkShaderModule CreateShader(byte[] bytecode)
        {
            VkShaderModuleCreateInfo smci = VkShaderModuleCreateInfo.New();
            fixed (byte* byteCodePtr = bytecode)
            {
                smci.pCode = (uint*)byteCodePtr;
                smci.codeSize = new UIntPtr((uint)bytecode.Length);
                vkCreateShaderModule(_device, ref smci, null, out VkShaderModule module);
                return module;
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