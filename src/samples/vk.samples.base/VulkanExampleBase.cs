// This code has been adapted from the "Vulkan" C++ example repository, by Sascha Willems.
// It is a direct translation from the original C++ code and style, with as little transformation as possible.

// Original file: base/vulkanexamplebase.cpp/h

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
using System.Runtime.InteropServices;
using System.Text;
using Vulkan;
using static Vulkan.VulkanNative;
using System.Numerics;
using System.IO;
using Veldrid;
using Veldrid.Sdl2;

namespace Vk.Samples
{
    public unsafe class VulkanExampleBase
    {
        public FixedUtf8String title { get; set; } = "Vulkan Example";
        public FixedUtf8String Name { get; set; } = "VulkanExample";
        public Settings Settings { get; } = new Settings();
        public IntPtr WindowInstance { get; protected set; }
        public VkInstance Instance { get; protected set; }
        public VkPhysicalDevice physicalDevice { get; protected set; }
        public vksVulkanDevice vulkanDevice { get; protected set; }
        public VkPhysicalDeviceFeatures enabledFeatures { get; protected set; }
        public NativeList<IntPtr> EnabledExtensions { get; } = new NativeList<IntPtr>();
        public VkDevice device { get; protected set; }
        public VkQueue queue { get; protected set; }
        public VkFormat DepthFormat { get; protected set; }
        public VulkanSwapchain Swapchain { get; } = new VulkanSwapchain();
        public IntPtr Window { get; protected set; }
        public VkCommandPool cmdPool => _cmdPool;
        public uint width { get; protected set; } = 1280;
        public uint height { get; protected set; } = 720;
        public NativeList<VkCommandBuffer> drawCmdBuffers { get; protected set; } = new NativeList<VkCommandBuffer>();
        public VkRenderPass renderPass => _renderPass;
        public VkPipelineCache pipelineCache => _pipelineCache;
        public NativeList<VkFramebuffer> frameBuffers { get; protected set; } = new NativeList<VkFramebuffer>();
        public VkPhysicalDeviceMemoryProperties DeviceMemoryProperties { get; set; }
        public VkPhysicalDeviceProperties DeviceProperties { get; protected set; }
        public VkPhysicalDeviceFeatures DeviceFeatures { get; protected set; }
        public Sdl2Window NativeWindow { get; private set; }

        public NativeList<Semaphores> semaphores = new NativeList<Semaphores>(1, 1);
        public Semaphores* GetSemaphoresPtr() => (Semaphores*)semaphores.GetAddress(0);
        public DepthStencil DepthStencil;
        public VkSubmitInfo submitInfo;
        public NativeList<VkPipelineStageFlags> submitPipelineStages = CreateSubmitPipelineStages();
        private static NativeList<VkPipelineStageFlags> CreateSubmitPipelineStages()
            => new NativeList<VkPipelineStageFlags>() { VkPipelineStageFlags.ColorAttachmentOutput };
        protected VkRenderPass _renderPass;
        private VkPipelineCache _pipelineCache;
        private VkCommandPool _cmdPool;
        protected VkDescriptorPool descriptorPool;

        // Destination dimensions for resizing the window
        private uint destWidth;
        private uint destHeight;
        private bool viewUpdated;
        private int frameCounter;
        protected float frameTimer;
        protected bool paused = false;
        protected bool prepared;

        // Defines a frame rate independent timer value clamped from -1.0...1.0
        // For use in animations, rotations, etc.
        protected float timer = 0.0f;
        // Multiplier for speeding up (or slowing down) the global timer
        protected float timerSpeed = 0.25f;

        protected float zoom;
        protected float zoomSpeed = 50f;
        protected Vector3 rotation;
        protected float rotationSpeed = 1f;
        protected Vector3 cameraPos = new Vector3();
        protected Vector2 mousePos;

        protected Camera camera = new Camera();

        protected VkClearColorValue defaultClearColor = GetDefaultClearColor();
        private static VkClearColorValue GetDefaultClearColor()
            => new VkClearColorValue(0.025f, 0.025f, 0.025f, 1.0f);

        // fps timer (one second interval)
        float fpsTimer = 0.0f;
        protected bool enableTextOverlay = false;
        private uint lastFPS;
        private readonly FrameTimeAverager _frameTimeAverager = new FrameTimeAverager(666);
        protected uint currentBuffer;
        protected NativeList<VkShaderModule> shaderModules = new NativeList<VkShaderModule>();

        protected InputSnapshot snapshot;

        public void InitVulkan()
        {
            VkResult err;
            err = CreateInstance(false);
            if (err != VkResult.Success)
            {
                throw new InvalidOperationException("Could not create Vulkan instance. Error: " + err);
            }

            if (Settings.Validation)
            {

            }

            // Physical Device
            uint gpuCount = 0;
            Util.CheckResult(vkEnumeratePhysicalDevices(Instance, &gpuCount, null));
            Debug.Assert(gpuCount > 0);
            // Enumerate devices
            IntPtr* physicalDevices = stackalloc IntPtr[(int)gpuCount];
            err = vkEnumeratePhysicalDevices(Instance, &gpuCount, (VkPhysicalDevice*)physicalDevices);
            if (err != VkResult.Success)
            {
                throw new InvalidOperationException("Could not enumerate physical devices.");
            }

            // GPU selection

            // Select physical Device to be used for the Vulkan example
            // Defaults to the first Device unless specified by command line

            uint selectedDevice = 0;
            // TODO: Implement arg parsing, etc.

            physicalDevice = ((VkPhysicalDevice*)physicalDevices)[selectedDevice];

            // Store properties (including limits) and features of the phyiscal Device
            // So examples can check against them and see if a feature is actually supported
            VkPhysicalDeviceProperties deviceProperties;
            vkGetPhysicalDeviceProperties(physicalDevice, &deviceProperties);
            DeviceProperties = deviceProperties;

            VkPhysicalDeviceFeatures deviceFeatures;
            vkGetPhysicalDeviceFeatures(physicalDevice, &deviceFeatures);
            DeviceFeatures = deviceFeatures;

            // Gather physical Device memory properties
            VkPhysicalDeviceMemoryProperties deviceMemoryProperties;
            vkGetPhysicalDeviceMemoryProperties(physicalDevice, &deviceMemoryProperties);
            DeviceMemoryProperties = deviceMemoryProperties;

            // Derived examples can override this to set actual features (based on above readings) to enable for logical device creation
            getEnabledFeatures();

            // Vulkan Device creation
            // This is handled by a separate class that gets a logical Device representation
            // and encapsulates functions related to a Device
            vulkanDevice = new vksVulkanDevice(physicalDevice);
            VkResult res = vulkanDevice.CreateLogicalDevice(enabledFeatures, EnabledExtensions);
            if (res != VkResult.Success)
            {
                throw new InvalidOperationException("Could not create Vulkan Device.");
            }
            device = vulkanDevice.LogicalDevice;

            // Get a graphics queue from the Device
            VkQueue queue;
            vkGetDeviceQueue(device, vulkanDevice.QFIndices.Graphics, 0, &queue);
            this.queue = queue;

            // Find a suitable depth format
            VkFormat depthFormat;
            uint validDepthFormat = Tools.getSupportedDepthFormat(physicalDevice, &depthFormat);
            Debug.Assert(validDepthFormat == True);
            DepthFormat = depthFormat;

            Swapchain.Connect(Instance, physicalDevice, device);

            // Create synchronization objects
            VkSemaphoreCreateInfo semaphoreCreateInfo = Initializers.semaphoreCreateInfo();
            // Create a semaphore used to synchronize image presentation
            // Ensures that the image is displayed before we start submitting new commands to the queu
            Util.CheckResult(vkCreateSemaphore(device, &semaphoreCreateInfo, null, &GetSemaphoresPtr()->PresentComplete));
            // Create a semaphore used to synchronize command submission
            // Ensures that the image is not presented until all commands have been sumbitted and executed
            Util.CheckResult(vkCreateSemaphore(device, &semaphoreCreateInfo, null, &GetSemaphoresPtr()->RenderComplete));
            // Create a semaphore used to synchronize command submission
            // Ensures that the image is not presented until all commands for the text overlay have been sumbitted and executed
            // Will be inserted after the render complete semaphore if the text overlay is enabled
            Util.CheckResult(vkCreateSemaphore(device, &semaphoreCreateInfo, null, &GetSemaphoresPtr()->TextOverlayComplete));

            // Set up submit info structure
            // Semaphores will stay the same during application lifetime
            // Command buffer submission info is set by each example
            submitInfo = Initializers.SubmitInfo();
            submitInfo.pWaitDstStageMask = (VkPipelineStageFlags*)submitPipelineStages.Data;
            submitInfo.waitSemaphoreCount = 1;
            submitInfo.pWaitSemaphores = &GetSemaphoresPtr()->PresentComplete;
            submitInfo.signalSemaphoreCount = 1;
            submitInfo.pSignalSemaphores = &GetSemaphoresPtr()->RenderComplete;
        }

        protected virtual void getEnabledFeatures()
        {
            // Used in some derived classes.
        }

        private VkResult CreateInstance(bool enableValidation)
        {
            Settings.Validation = enableValidation;

            VkApplicationInfo appInfo = new VkApplicationInfo()
            {
                sType = VkStructureType.ApplicationInfo,
                apiVersion = new Version(1, 0, 0),
                pApplicationName = Name,
                pEngineName = Name,
            };

            NativeList<IntPtr> instanceExtensions = new NativeList<IntPtr>(2);
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

            VkInstanceCreateInfo instanceCreateInfo = VkInstanceCreateInfo.New();
            instanceCreateInfo.pApplicationInfo = &appInfo;

            if (instanceExtensions.Count > 0)
            {
                if (enableValidation)
                {
                    instanceExtensions.Add(Strings.VK_EXT_DEBUG_REPORT_EXTENSION_NAME);
                }
                instanceCreateInfo.enabledExtensionCount = instanceExtensions.Count;
                instanceCreateInfo.ppEnabledExtensionNames = (byte**)instanceExtensions.Data;
            }


            if (enableValidation)
            {
                NativeList<IntPtr> enabledLayerNames = new NativeList<IntPtr>(1);
                enabledLayerNames.Add(Strings.StandardValidationLayeName);
                instanceCreateInfo.enabledLayerCount = enabledLayerNames.Count;
                instanceCreateInfo.ppEnabledLayerNames = (byte**)enabledLayerNames.Data;
            }

            VkInstance instance;
            VkResult result = vkCreateInstance(&instanceCreateInfo, null, &instance);
            Instance = instance;
            return result;
        }

        public IntPtr SetupWin32Window()
        {
            WindowInstance = Process.GetCurrentProcess().SafeHandle.DangerousGetHandle();
            NativeWindow = new Sdl2Window(Name, 50, 50, 1280, 720, SDL_WindowFlags.Resizable, threadedProcessing: false);
            NativeWindow.X = 50;
            NativeWindow.Y = 50;
            NativeWindow.Visible = true;
            NativeWindow.Resized += OnNativeWindowResized;
            NativeWindow.MouseWheel += OnMouseWheel;
            NativeWindow.MouseMove += OnMouseMove;
            NativeWindow.MouseDown += OnMouseDown;
            NativeWindow.KeyDown += OnKeyDown;
            Window = NativeWindow.Handle;
            return NativeWindow.Handle;
        }

        private void OnKeyDown(KeyEvent e)
        {
            if (e.Key == Key.F4 && (e.Modifiers & ModifierKeys.Alt) != 0 || e.Key == Key.Escape)
            {
                NativeWindow.Close();
            }

            keyPressed(e.Key);
        }

        private void OnMouseDown(MouseEvent e)
        {
            if (e.Down)
            {
                mousePos = new Vector2(snapshot.MousePosition.X, snapshot.MousePosition.Y);
            }
        }

        private void OnMouseMove(MouseMoveEventArgs e)
        {
            if (e.State.IsButtonDown(MouseButton.Right))
            {
                int posx = (int)e.MousePosition.X;
                int posy = (int)e.MousePosition.Y;
                zoom += (mousePos.Y - posy) * .005f * zoomSpeed;
                camera.translate(new Vector3(-0.0f, 0.0f, (mousePos.Y - posy) * .005f * zoomSpeed));
                mousePos = new Vector2(posx, posy);
                viewUpdated = true;
            }
            if (e.State.IsButtonDown(MouseButton.Left))
            {
                int posx = (int)e.MousePosition.X;
                int posy = (int)e.MousePosition.Y;
                rotation.X += (mousePos.Y - posy) * 1.25f * rotationSpeed;
                rotation.Y -= (mousePos.X - posx) * 1.25f * rotationSpeed;
                camera.rotate(new Vector3((mousePos.Y - posy) * camera.rotationSpeed, -(mousePos.X - posx) * camera.rotationSpeed, 0.0f));
                mousePos = new Vector2(posx, posy);
                viewUpdated = true;
            }
            if (e.State.IsButtonDown(MouseButton.Middle))
            {
                int posx = (int)e.MousePosition.X;
                int posy = (int)e.MousePosition.Y;
                cameraPos.X -= (mousePos.X - posx) * 0.01f;
                cameraPos.Y -= (mousePos.Y - posy) * 0.01f;
                camera.translate(new Vector3(-(mousePos.X - posx) * 0.01f, -(mousePos.Y - posy) * 0.01f, 0.0f));
                viewUpdated = true;
                mousePos.X = posx;
                mousePos.Y = posy;
            }
        }

        private void OnMouseWheel(MouseWheelEventArgs e)
        {
            var wheelDelta = e.WheelDelta;
            zoom += wheelDelta * 0.005f * zoomSpeed;
            camera.translate(new Vector3(0.0f, 0.0f, wheelDelta * 0.005f * zoomSpeed));
            viewUpdated = true;
        }

        private void OnNativeWindowResized()
        {
            windowResize();
        }

        public void InitSwapchain()
        {
            Swapchain.InitSurface(NativeWindow.SdlWindowHandle);
        }

        public virtual void Prepare()
        {
            if (vulkanDevice.EnableDebugMarkers)
            {
                // vks::debugmarker::setup(Device);
            }

            CreateCommandPool();
            SetupSwapChain();
            createCommandBuffers();
            SetupDepthStencil();
            SetupRenderPass();
            CreatePipelineCache();
            SetupFrameBuffer();

            /* TODO: Implement text rendering
            if (enableTextOverlay)
            {
                // Load the text rendering shaders
                std::vector<VkPipelineShaderStageCreateInfo> shaderStages;
                shaderStages.push_back(loadShader(getAssetPath() + "shaders/base/textoverlay.vert.spv", VK_SHADER_STAGE_VERTEX_BIT));
                shaderStages.push_back(loadShader(getAssetPath() + "shaders/base/textoverlay.frag.spv", VK_SHADER_STAGE_FRAGMENT_BIT));
                textOverlay = new VulkanTextOverlay(
                    vulkanDevice,
                    queue,
                    frameBuffers,
                    swapChain.colorFormat,
                    depthFormat,
                    &width,
                    &height,
                    shaderStages
                    );
                updateTextOverlay();
            }
            */
        }

        protected void prepareFrame()
        {
            // Acquire the next image from the swap chaing
            Util.CheckResult(Swapchain.AcquireNextImage(semaphores[0].PresentComplete, ref currentBuffer));
        }

        protected virtual void SetupRenderPass()
        {
            using (NativeList<VkAttachmentDescription> attachments = new NativeList<VkAttachmentDescription>())
            {
                attachments.Count = 2;
                // Color attachment
                attachments[0] = new VkAttachmentDescription();
                attachments[0].format = Swapchain.ColorFormat;
                attachments[0].samples = VkSampleCountFlags.Count1;
                attachments[0].loadOp = VkAttachmentLoadOp.Clear;
                attachments[0].storeOp = VkAttachmentStoreOp.Store;
                attachments[0].stencilLoadOp = VkAttachmentLoadOp.DontCare;
                attachments[0].stencilStoreOp = VkAttachmentStoreOp.DontCare;
                attachments[0].initialLayout = VkImageLayout.Undefined;
                attachments[0].finalLayout = VkImageLayout.PresentSrcKHR;
                // Depth attachment
                attachments[1] = new VkAttachmentDescription();
                attachments[1].format = DepthFormat;
                attachments[1].samples = VkSampleCountFlags.Count1;
                attachments[1].loadOp = VkAttachmentLoadOp.Clear;
                attachments[1].storeOp = VkAttachmentStoreOp.Store;
                attachments[1].stencilLoadOp = VkAttachmentLoadOp.DontCare;
                attachments[1].stencilStoreOp = VkAttachmentStoreOp.DontCare;
                attachments[1].initialLayout = VkImageLayout.Undefined;
                attachments[1].finalLayout = VkImageLayout.DepthStencilAttachmentOptimal;

                VkAttachmentReference colorReference = new VkAttachmentReference();
                colorReference.attachment = 0;
                colorReference.layout = VkImageLayout.ColorAttachmentOptimal;

                VkAttachmentReference depthReference = new VkAttachmentReference();
                depthReference.attachment = 1;
                depthReference.layout = VkImageLayout.DepthStencilAttachmentOptimal;

                VkSubpassDescription subpassDescription = new VkSubpassDescription();
                subpassDescription.pipelineBindPoint = VkPipelineBindPoint.Graphics;
                subpassDescription.colorAttachmentCount = 1;
                subpassDescription.pColorAttachments = &colorReference;
                subpassDescription.pDepthStencilAttachment = &depthReference;
                subpassDescription.inputAttachmentCount = 0;
                subpassDescription.pInputAttachments = null;
                subpassDescription.preserveAttachmentCount = 0;
                subpassDescription.pPreserveAttachments = null;
                subpassDescription.pResolveAttachments = null;

                // Subpass dependencies for layout transitions
                using (NativeList<VkSubpassDependency> dependencies = new NativeList<VkSubpassDependency>(2))
                {
                    dependencies.Count = 2;

                    dependencies[0].srcSubpass = SubpassExternal;
                    dependencies[0].dstSubpass = 0;
                    dependencies[0].srcStageMask = VkPipelineStageFlags.BottomOfPipe;
                    dependencies[0].dstStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
                    dependencies[0].srcAccessMask = VkAccessFlags.MemoryRead;
                    dependencies[0].dstAccessMask = (VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite);
                    dependencies[0].dependencyFlags = VkDependencyFlags.ByRegion;

                    dependencies[1].srcSubpass = 0;
                    dependencies[1].dstSubpass = SubpassExternal;
                    dependencies[1].srcStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
                    dependencies[1].dstStageMask = VkPipelineStageFlags.BottomOfPipe;
                    dependencies[1].srcAccessMask = (VkAccessFlags.ColorAttachmentRead | VkAccessFlags.ColorAttachmentWrite);
                    dependencies[1].dstAccessMask = VkAccessFlags.MemoryRead;
                    dependencies[1].dependencyFlags = VkDependencyFlags.ByRegion;

                    VkRenderPassCreateInfo renderPassInfo = new VkRenderPassCreateInfo();
                    renderPassInfo.sType = VkStructureType.RenderPassCreateInfo;
                    renderPassInfo.attachmentCount = attachments.Count;
                    renderPassInfo.pAttachments = (VkAttachmentDescription*)attachments.Data.ToPointer();
                    renderPassInfo.subpassCount = 1;
                    renderPassInfo.pSubpasses = &subpassDescription;
                    renderPassInfo.dependencyCount = dependencies.Count;
                    renderPassInfo.pDependencies = (VkSubpassDependency*)dependencies.Data;

                    Util.CheckResult(vkCreateRenderPass(device, &renderPassInfo, null, out _renderPass));
                }
            }
        }

        private void CreatePipelineCache()
        {
            VkPipelineCacheCreateInfo pipelineCacheCreateInfo = VkPipelineCacheCreateInfo.New();
            Util.CheckResult(vkCreatePipelineCache(device, ref pipelineCacheCreateInfo, null, out _pipelineCache));
        }

        protected virtual void SetupFrameBuffer()
        {
            using (NativeList<VkImageView> attachments = new NativeList<VkImageView>(2))
            {
                attachments.Count = 2;
                // Depth/Stencil attachment is the same for all frame buffers
                attachments[1] = DepthStencil.View;

                VkFramebufferCreateInfo frameBufferCreateInfo = VkFramebufferCreateInfo.New();
                frameBufferCreateInfo.renderPass = renderPass;
                frameBufferCreateInfo.attachmentCount = 2;
                frameBufferCreateInfo.pAttachments = (VkImageView*)attachments.Data;
                frameBufferCreateInfo.width = width;
                frameBufferCreateInfo.height = height;
                frameBufferCreateInfo.layers = 1;

                // Create frame buffers for every swap chain image
                frameBuffers.Count = (Swapchain.ImageCount);
                for (uint i = 0; i < frameBuffers.Count; i++)
                {
                    attachments[0] = Swapchain.Buffers[i].View;
                    Util.CheckResult(vkCreateFramebuffer(device, ref frameBufferCreateInfo, null, (VkFramebuffer*)Unsafe.AsPointer(ref frameBuffers[i])));
                }
            }
        }

        private void SetupSwapChain()
        {
            uint width, height;
            Swapchain.Create(&width, &height, Settings.VSync);

            this.width = width;
            this.height = height;
        }

        protected void submitFrame()
        {
            bool submitTextOverlay = false;
            /*
            if (submitTextOverlay)
            {
                // Wait for color attachment output to finish before rendering the text overlay
                VkPipelineStageFlags stageFlags = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT;
                submitInfo.pWaitDstStageMask = &stageFlags;

                // Set semaphores
                // Wait for render complete semaphore
                submitInfo.waitSemaphoreCount = 1;
                submitInfo.pWaitSemaphores = &semaphores.renderComplete;
                // Signal ready with text overlay complete semaphpre
                submitInfo.signalSemaphoreCount = 1;
                submitInfo.pSignalSemaphores = &semaphores.textOverlayComplete;

                // Submit current text overlay command buffer
                submitInfo.commandBufferCount = 1;
                submitInfo.pCommandBuffers = &textOverlay->cmdBuffers[currentBuffer];
                Util.CheckResult(vkQueueSubmit(queue, 1, &submitInfo, VK_NULL_HANDLE));

                // Reset stage mask
                submitInfo.pWaitDstStageMask = &submitPipelineStages;
                // Reset wait and signal semaphores for rendering next frame
                // Wait for swap chain presentation to finish
                submitInfo.waitSemaphoreCount = 1;
                submitInfo.pWaitSemaphores = &semaphores.presentComplete;
                // Signal ready with offscreen semaphore
                submitInfo.signalSemaphoreCount = 1;
                submitInfo.pSignalSemaphores = &semaphores.renderComplete;
            }
            */

            Util.CheckResult(Swapchain.QueuePresent(queue, currentBuffer, submitTextOverlay ? semaphores[0].TextOverlayComplete : semaphores[0].RenderComplete));

            Util.CheckResult(vkQueueWaitIdle(queue));
        }

        private void CreateCommandPool()
        {
            VkCommandPoolCreateInfo cmdPoolInfo = VkCommandPoolCreateInfo.New();
            cmdPoolInfo.queueFamilyIndex = Swapchain.QueueNodeIndex;
            cmdPoolInfo.flags = VkCommandPoolCreateFlags.ResetCommandBuffer;
            Util.CheckResult(vkCreateCommandPool(device, &cmdPoolInfo, null, out _cmdPool));
        }

        protected void createCommandBuffers()
        {
            // Create one command buffer for each swap chain image and reuse for rendering
            drawCmdBuffers.Resize(Swapchain.ImageCount);
            drawCmdBuffers.Count = Swapchain.ImageCount;

            VkCommandBufferAllocateInfo cmdBufAllocateInfo =
                Initializers.CommandBufferAllocateInfo(cmdPool, VkCommandBufferLevel.Primary, drawCmdBuffers.Count);

            Util.CheckResult(vkAllocateCommandBuffers(device, ref cmdBufAllocateInfo, (VkCommandBuffer*)drawCmdBuffers.Data));
        }

        protected bool checkCommandBuffers()
        {
            foreach (var cmdBuffer in drawCmdBuffers)
            {
                if (cmdBuffer == NullHandle)
                {
                    return false;
                }
            }
            return true;
        }

        protected void destroyCommandBuffers()
        {
            vkFreeCommandBuffers(device, cmdPool, drawCmdBuffers.Count, drawCmdBuffers.Data);
        }

        protected virtual void SetupDepthStencil()
        {
            VkImageCreateInfo image = VkImageCreateInfo.New();
            image.imageType = VkImageType.Image2D;
            image.format = DepthFormat;
            image.extent = new VkExtent3D() { width = width, height = height, depth = 1 };
            image.mipLevels = 1;
            image.arrayLayers = 1;
            image.samples = VkSampleCountFlags.Count1;
            image.tiling = VkImageTiling.Optimal;
            image.usage = (VkImageUsageFlags.DepthStencilAttachment | VkImageUsageFlags.TransferSrc);
            image.flags = 0;

            VkMemoryAllocateInfo mem_alloc = VkMemoryAllocateInfo.New();
            mem_alloc.allocationSize = 0;
            mem_alloc.memoryTypeIndex = 0;

            VkImageViewCreateInfo depthStencilView = VkImageViewCreateInfo.New();
            depthStencilView.viewType = VkImageViewType.Image2D;
            depthStencilView.format = DepthFormat;
            depthStencilView.flags = 0;
            depthStencilView.subresourceRange = new VkImageSubresourceRange();
            depthStencilView.subresourceRange.aspectMask = (VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil);
            depthStencilView.subresourceRange.baseMipLevel = 0;
            depthStencilView.subresourceRange.levelCount = 1;
            depthStencilView.subresourceRange.baseArrayLayer = 0;
            depthStencilView.subresourceRange.layerCount = 1;

            Util.CheckResult(vkCreateImage(device, &image, null, out DepthStencil.Image));
            vkGetImageMemoryRequirements(device, DepthStencil.Image, out VkMemoryRequirements memReqs);
            mem_alloc.allocationSize = memReqs.size;
            mem_alloc.memoryTypeIndex = vulkanDevice.getMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);
            Util.CheckResult(vkAllocateMemory(device, &mem_alloc, null, out DepthStencil.Mem));
            Util.CheckResult(vkBindImageMemory(device, DepthStencil.Image, DepthStencil.Mem, 0));

            depthStencilView.image = DepthStencil.Image;
            Util.CheckResult(vkCreateImageView(device, ref depthStencilView, null, out DepthStencil.View));
        }

        public void RenderLoop()
        {
            destWidth = width;
            destHeight = height;
            while (NativeWindow.Exists)
            {
                var tStart = DateTime.Now;
                if (viewUpdated)
                {
                    viewUpdated = false;
                    viewChanged();
                }

                snapshot = NativeWindow.PumpEvents();

                if (!NativeWindow.Exists)
                {
                    // Exit early if the window was closed this frame.
                    break;
                }

                render();
                frameCounter++;
                var tEnd = DateTime.Now;
                var tDiff = tEnd - tStart;
                frameTimer = (float)tDiff.TotalMilliseconds / 1000.0f;
                _frameTimeAverager.AddTime(tDiff.TotalMilliseconds);
                /*
                camera.update(frameTimer);
                if (camera.moving())
                {
                    viewUpdated = true;
                }
                */
                // Convert to clamped timer value
                if (!paused)
                {
                    timer += timerSpeed * frameTimer;
                    if (timer > 1.0)
                    {
                        timer -= 1.0f;
                    }
                }
                fpsTimer += (float)tDiff.TotalMilliseconds * 1000f;
                if (fpsTimer > 1000.0f)
                {
                    if (!enableTextOverlay)
                    {
                        NativeWindow.Title = getWindowTitle();
                    }
                    lastFPS = (uint)(1.0f / frameTimer);
                    // updateTextOverlay();
                    fpsTimer = 0.0f;
                    frameCounter = 0;
                }
            }
            // Flush device to make sure all resources can be freed 
            vkDeviceWaitIdle(device);
        }

        protected virtual void viewChanged()
        {
        }

        private string getWindowTitle()
        {
            var dp = DeviceProperties;
            string device = Encoding.UTF8.GetString(dp.deviceName, (int)MaxPhysicalDeviceNameSize);
            int firstNull = device.IndexOf('\0');
            device = device.Remove(firstNull);
            string windowTitle;
            windowTitle = title + " - " + device;
            if (!enableTextOverlay)
            {
                windowTitle += " - " + _frameTimeAverager.CurrentAverageFramesPerSecond.ToString("000.0 fps / ") + _frameTimeAverager.CurrentAverageFrameTime.ToString("#00.00 ms");
            }
            return windowTitle;
        }

        protected virtual void render() { }

        void windowResize()
        {
            if (!prepared)
            {
                return;
            }
            prepared = false;

            // Ensure all operations on the device have been finished before destroying resources
            vkDeviceWaitIdle(device);

            // Recreate swap chain
            width = destWidth;
            height = destHeight;
            SetupSwapChain();

            // Recreate the frame buffers

            vkDestroyImageView(device, DepthStencil.View, null);
            vkDestroyImage(device, DepthStencil.Image, null);
            vkFreeMemory(device, DepthStencil.Mem, null);
            SetupDepthStencil();

            for (uint i = 0; i < frameBuffers.Count; i++)
            {
                vkDestroyFramebuffer(device, frameBuffers[i], null);
            }
            SetupFrameBuffer();

            // Command buffers need to be recreated as they may store
            // references to the recreated frame buffer
            destroyCommandBuffers();
            createCommandBuffers();
            buildCommandBuffers();

            vkDeviceWaitIdle(device);

            if (enableTextOverlay)
            {
                //textOverlay->reallocateCommandBuffers();
                //updateTextOverlay();
            }

            // camera.updateAspectRatio((float)width / (float)height);

            // Notify derived class
            windowResized();
            viewChanged();

            prepared = true;
        }

        protected VkPipelineShaderStageCreateInfo loadShader(string fileName, VkShaderStageFlags stage)
        {
            VkPipelineShaderStageCreateInfo shaderStage = VkPipelineShaderStageCreateInfo.New();
            shaderStage.stage = stage;
            shaderStage.module = Tools.loadShader(fileName, device, stage);
            shaderStage.pName = Strings.main; // todo : make param
            Debug.Assert(shaderStage.module.Handle != 0);
            shaderModules.Add(shaderStage.module);
            return shaderStage;
        }

        protected VkCommandBuffer createCommandBuffer(VkCommandBufferLevel level, bool begin)
        {
            VkCommandBuffer cmdBuffer;

            VkCommandBufferAllocateInfo cmdBufAllocateInfo = Initializers.CommandBufferAllocateInfo(cmdPool, level, 1);

            Util.CheckResult(vkAllocateCommandBuffers(device, &cmdBufAllocateInfo, out cmdBuffer));

            // If requested, also start the new command buffer
            if (begin)
            {
                VkCommandBufferBeginInfo cmdBufInfo = Initializers.commandBufferBeginInfo();
                Util.CheckResult(vkBeginCommandBuffer(cmdBuffer, &cmdBufInfo));
            }

            return cmdBuffer;
        }

        protected void flushCommandBuffer(VkCommandBuffer commandBuffer, VkQueue queue, bool free)
        {
            if (commandBuffer == NullHandle)
            {
                return;
            }

            Util.CheckResult(vkEndCommandBuffer(commandBuffer));

            VkSubmitInfo submitInfo = VkSubmitInfo.New();
            submitInfo.commandBufferCount = 1;
            submitInfo.pCommandBuffers = &commandBuffer;

            Util.CheckResult(vkQueueSubmit(queue, 1, &submitInfo, VkFence.Null));
            Util.CheckResult(vkQueueWaitIdle(queue));

            if (free)
            {
                vkFreeCommandBuffers(device, cmdPool, 1, &commandBuffer);
            }
        }

        protected string getAssetPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "data/");
        }

        public void ExampleMain()
        {
            InitVulkan();
            SetupWin32Window();
            InitSwapchain();
            Prepare();
            RenderLoop();
        }

        protected virtual void windowResized()
        {
        }

        protected virtual void buildCommandBuffers()
        {
        }

        protected virtual void keyPressed(Key key)
        {
        }
    }

    public struct Semaphores
    {
        public VkSemaphore PresentComplete;
        public VkSemaphore RenderComplete;
        public VkSemaphore TextOverlayComplete;
    }

    public struct DepthStencil
    {
        public VkImage Image;
        public VkDeviceMemory Mem;
        public VkImageView View;
    }

    public class Settings
    {
        public bool Validation { get; set; } = false;
        public bool Fullscreen { get; set; } = false;
        public bool VSync { get; set; } = false;
    }
}
