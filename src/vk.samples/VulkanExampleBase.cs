using OpenTK;
using OpenTK.Graphics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public unsafe class VulkanExampleBase
    {
        public FixedUtf8String Title { get; } = "Vulkan Example";
        public FixedUtf8String Name { get; } = "VulkanExample";
        public Settings Settings { get; } = new Settings();
        public IntPtr WindowInstance { get; protected set; }
        public VkInstance Instance { get; protected set; }
        public VkPhysicalDevice PhysicalDevice { get; protected set; }
        public VulkanDevice VulkanDevice { get; protected set; }
        public VkPhysicalDeviceFeatures EnabledFeatures { get; protected set; }
        public NativeList<IntPtr> EnabledExtensions { get; } = new NativeList<IntPtr>();
        public VkDevice Device { get; protected set; }
        public VkQueue Queue { get; protected set; }
        public VkFormat DepthFormat { get; protected set; }
        public VulkanSwapchain Swapchain { get; } = new VulkanSwapchain();
        public IntPtr Window { get; protected set; }
        public VkCommandPool CmdPool { get; protected set; }
        public uint Width { get; protected set; }
        public uint Height { get; protected set; }

        public Semaphores Semaphores;
        public DepthStencil DepthStencil;
        public VkSubmitInfo SubmitInfo;
        public NativeList<VkPipelineStageFlagBits> submitPipelineStages = new NativeList<VkPipelineStageFlagBits>(1);

        // Destination dimensions for resizing the window
        private uint destWidth;
        private uint destHeight;
        private bool viewUpdated;
        private int frameCounter;
        private float frameTimer;
        private bool paused;
        protected bool prepared;

        // Defines a frame rate independent timer value clamped from -1.0...1.0
        // For use in animations, rotations, etc.
        float timer = 0.0f;
        // Multiplier for speeding up (or slowing down) the global timer
        float timerSpeed = 0.25f;

        // fps timer (one second interval)
        float fpsTimer = 0.0f;
        private bool enableTextOverlay;
        private uint lastFPS;
        private readonly FrameTimeAverager _frameTimeAverager = new FrameTimeAverager(666);

        public NativeList<VkCommandBuffer> DrawCmdBuffers { get; protected set; } = new NativeList<VkCommandBuffer>();
        public VkRenderPass RenderPass { get; protected set; }
        public VkPipelineCache PipelineCache { get; set; }
        public NativeList<VkFramebuffer> Framebuffers { get; protected set; } = new NativeList<VkFramebuffer>();
        public VkPhysicalDeviceMemoryProperties DeviceMemoryProperties { get; set; }
        public VkPhysicalDeviceProperties DeviceProperties { get; protected set; }
        public VkPhysicalDeviceFeatures DeviceFeatures { get; protected set; }
        public NativeWindow NativeWindow { get; private set; }

        public void InitVulkan()
        {
            VkResult err;
            err = CreateInstance(false);
            if (err != VkResult.Success)
            {
                throw new InvalidOperationException("Could not create Vulkan instance.");
            }

            // TODO:
            // if (Settings.Validation)

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

            PhysicalDevice = ((VkPhysicalDevice*)physicalDevices)[selectedDevice];

            // Vulkan Device creation
            // This is handled by a separate class that gets a logical Device representation
            // and encapsulates functions related to a Device
            VulkanDevice = new VulkanDevice(PhysicalDevice);
            VkResult res = VulkanDevice.CreateLogicalDevice(EnabledFeatures, EnabledExtensions);
            if (res != VkResult.Success)
            {
                throw new InvalidOperationException("Could not create Vulkan Device.");
            }
            Device = VulkanDevice.LogicalDevice;

            // todo: remove
            // Store properties (including limits) and features of the phyiscal Device
            // So examples can check against them and see if a feature is actually supported
            VkPhysicalDeviceProperties deviceProperties;
            vkGetPhysicalDeviceProperties(PhysicalDevice, &deviceProperties);
            DeviceProperties = deviceProperties;

            VkPhysicalDeviceFeatures deviceFeatures;
            vkGetPhysicalDeviceFeatures(PhysicalDevice, &deviceFeatures);
            DeviceFeatures = deviceFeatures;

            // Gather physical Device memory properties
            VkPhysicalDeviceMemoryProperties deviceMemoryProperties;
            vkGetPhysicalDeviceMemoryProperties(PhysicalDevice, &deviceMemoryProperties);
            DeviceMemoryProperties = deviceMemoryProperties;

            // Get a graphics queue from the Device
            VkQueue queue;
            vkGetDeviceQueue(Device, VulkanDevice.QFIndices.Graphics, 0, &queue);
            Queue = queue;

            // Find a suitable depth format
            VkFormat depthFormat;
            uint validDepthFormat = Tools.getSupportedDepthFormat(PhysicalDevice, &depthFormat);
            Debug.Assert(validDepthFormat == True);
            DepthFormat = depthFormat;

            Swapchain.Connect(Instance, PhysicalDevice, Device);

            Semaphores semaphores;
            // Create synchronization objects
            VkSemaphoreCreateInfo semaphoreCreateInfo = Initializers.SemaphoreCreateInfo();
            // Create a semaphore used to synchronize image presentation
            // Ensures that the image is displayed before we start submitting new commands to the queu
            Util.CheckResult(vkCreateSemaphore(Device, &semaphoreCreateInfo, null, &semaphores.PresentComplete));
            // Create a semaphore used to synchronize command submission
            // Ensures that the image is not presented until all commands have been sumbitted and executed
            Util.CheckResult(vkCreateSemaphore(Device, &semaphoreCreateInfo, null, &semaphores.RenderComplete));
            // Create a semaphore used to synchronize command submission
            // Ensures that the image is not presented until all commands for the text overlay have been sumbitted and executed
            // Will be inserted after the render complete semaphore if the text overlay is enabled
            Util.CheckResult(vkCreateSemaphore(Device, &semaphoreCreateInfo, null, &semaphores.TextOverlayComplete));
            Semaphores = semaphores;

            // Set up submit info structure
            // Semaphores will stay the same during application lifetime
            // Command buffer submission info is set by each example
            var submitInfo = Initializers.SubmitInfo();
            submitInfo.pWaitDstStageMask = (uint*)submitPipelineStages.Data;
            submitInfo.waitSemaphoreCount = 1;
            submitInfo.pWaitSemaphores = &semaphores.PresentComplete;
            submitInfo.signalSemaphoreCount = 1;
            submitInfo.pSignalSemaphores = &semaphores.RenderComplete;
            SubmitInfo = submitInfo;
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

            byte** instanceExtensions = stackalloc byte*[2];
            instanceExtensions[0] = Strings.VK_KHR_SURFACE_EXTENSION_NAME;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                instanceExtensions[1] = Strings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                instanceExtensions[1] = Strings.VK_KHR_XCB_SURFACE_EXTENSION_NAME;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            VkInstanceCreateInfo instanceCreateInfo = new VkInstanceCreateInfo()
            {
                sType = VkStructureType.InstanceCreateInfo,
                pApplicationInfo = &appInfo,
                enabledExtensionCount = 2,
                ppEnabledExtensionNames = instanceExtensions
            };

            // TODO: VALIDATION

            VkInstance instance;
            VkResult result = vkCreateInstance(&instanceCreateInfo, null, &instance);
            Instance = instance;
            return result;
        }

        public IntPtr SetupWin32Window()
        {
            WindowInstance = Process.GetCurrentProcess().SafeHandle.DangerousGetHandle();
            NativeWindow = new NativeWindow(960, 540, Name, GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default);
            NativeWindow.Visible = true;
            NativeWindow.Resize += OnNativeWindowResized;
            Window = NativeWindow.WindowInfo.Handle;
            return NativeWindow.WindowInfo.Handle;
        }

        private void OnNativeWindowResized(object sender, EventArgs e)
        {
            windowResize();
        }

        public void InitSwapchain()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Swapchain.InitSurface(WindowInstance, Window);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public virtual void Prepare()
        {
            if (VulkanDevice.EnableDebugMarkers)
            {
                // vks::debugmarker::setup(Device);
            }

            CreateCommandPool();
            SetupSwapChain();
            CreateCommandBuffers();
            SetupDepthStencil();
            SetupRenderPass();
            CreatePipelineCache();
            setupFrameBuffer();

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

        protected virtual void SetupRenderPass()
        {
            using (NativeList<VkAttachmentDescription> attachments = new NativeList<VkAttachmentDescription>())
            {
                attachments.Count = 2;
                // Color attachment
                attachments[0].format = Swapchain.ColorFormat;
                attachments[0].samples = VkSampleCountFlagBits._1;
                attachments[0].loadOp = VkAttachmentLoadOp.Clear;
                attachments[0].storeOp = VkAttachmentStoreOp.Store;
                attachments[0].stencilLoadOp = VkAttachmentLoadOp.DontCare;
                attachments[0].stencilStoreOp = VkAttachmentStoreOp.DontCare;
                attachments[0].initialLayout = VkImageLayout.Undefined;
                attachments[0].finalLayout = Hacks.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR;
                // Depth attachment
                attachments[1].format = DepthFormat;
                attachments[1].samples = VkSampleCountFlagBits._1;
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
                    dependencies[0].srcStageMask = (uint)VkPipelineStageFlagBits.BottomOfPipe;
                    dependencies[0].dstStageMask = (uint)VkPipelineStageFlagBits.ColorAttachmentOutput;
                    dependencies[0].srcAccessMask = (uint)VkAccessFlagBits.MemoryRead;
                    dependencies[0].dstAccessMask = (uint)(VkAccessFlagBits.ColorAttachmentRead | VkAccessFlagBits.ColorAttachmentWrite);
                    dependencies[0].dependencyFlags = (uint)VkDependencyFlagBits.ByRegion;

                    dependencies[1].srcSubpass = 0;
                    dependencies[1].dstSubpass = SubpassExternal;
                    dependencies[1].srcStageMask = (uint)VkPipelineStageFlagBits.ColorAttachmentOutput;
                    dependencies[1].dstStageMask = (uint)VkPipelineStageFlagBits.BottomOfPipe;
                    dependencies[1].srcAccessMask = (uint)(VkAccessFlagBits.ColorAttachmentRead | VkAccessFlagBits.ColorAttachmentWrite);
                    dependencies[1].dstAccessMask = (uint)VkAccessFlagBits.MemoryRead;
                    dependencies[1].dependencyFlags = (uint)VkDependencyFlagBits.ByRegion;

                    VkRenderPassCreateInfo renderPassInfo = new VkRenderPassCreateInfo();
                    renderPassInfo.sType = VkStructureType.RenderPassCreateInfo;
                    renderPassInfo.attachmentCount = attachments.Count;
                    renderPassInfo.pAttachments = (VkAttachmentDescription*)attachments.Data.ToPointer();
                    renderPassInfo.subpassCount = 1;
                    renderPassInfo.pSubpasses = &subpassDescription;
                    renderPassInfo.dependencyCount = dependencies.Count;
                    renderPassInfo.pDependencies = (VkSubpassDependency*)dependencies.Data;

                    VkRenderPass renderPass;
                    Util.CheckResult(vkCreateRenderPass(Device, &renderPassInfo, null, &renderPass));
                    RenderPass = renderPass;
                }
            }
        }

        private void CreatePipelineCache()
        {
            VkPipelineCacheCreateInfo pipelineCacheCreateInfo = new VkPipelineCacheCreateInfo();
            pipelineCacheCreateInfo.sType = VkStructureType.PipelineCacheCreateInfo;
            VkPipelineCache pipelineCache;
            Util.CheckResult(vkCreatePipelineCache(Device, &pipelineCacheCreateInfo, null, &pipelineCache));
            PipelineCache = pipelineCache;
        }

        protected virtual void setupFrameBuffer()
        {
            using (NativeList<VkImageView> attachments = new NativeList<VkImageView>(2))
            {
                attachments.Count = 2;
                // Depth/Stencil attachment is the same for all frame buffers
                attachments[1] = DepthStencil.View;

                VkFramebufferCreateInfo frameBufferCreateInfo = new VkFramebufferCreateInfo();
                frameBufferCreateInfo.sType = VkStructureType.FramebufferCreateInfo;
                frameBufferCreateInfo.pNext = null;
                frameBufferCreateInfo.renderPass = RenderPass;
                frameBufferCreateInfo.attachmentCount = 2;
                frameBufferCreateInfo.pAttachments = (VkImageView*)attachments.Data;
                frameBufferCreateInfo.width = Width;
                frameBufferCreateInfo.height = Height;
                frameBufferCreateInfo.layers = 1;

                // Create frame buffers for every swap chain image
                Framebuffers.Resize(Swapchain.ImageCount);
                for (uint i = 0; i < Framebuffers.Count; i++)
                {
                    attachments[0] = Swapchain.Buffers[i].View;
                    Util.CheckResult(vkCreateFramebuffer(Device, &frameBufferCreateInfo, null, (VkFramebuffer*)Unsafe.AsPointer(ref Framebuffers[i])));
                }
            }
        }

        private void SetupSwapChain()
        {
            uint width, height;
            Swapchain.Create(&width, &height, Settings.VSync);

            Width = width;
            Height = height;
        }

        private void CreateCommandPool()
        {
            VkCommandPoolCreateInfo cmdPoolInfo = new VkCommandPoolCreateInfo();
            cmdPoolInfo.sType = VkStructureType.CommandPoolCreateInfo;
            cmdPoolInfo.queueFamilyIndex = Swapchain.QueueNodeIndex;
            cmdPoolInfo.flags = (uint)VkCommandPoolCreateFlagBits.ResetCommandBuffer;
            VkCommandPool cmdPool;
            Util.CheckResult(vkCreateCommandPool(Device, &cmdPoolInfo, null, &cmdPool));
            CmdPool = cmdPool;
        }

        private void CreateCommandBuffers()
        {
            // Create one command buffer for each swap chain image and reuse for rendering
            DrawCmdBuffers.Resize(Swapchain.ImageCount);
            DrawCmdBuffers.Count = Swapchain.ImageCount;

            VkCommandBufferAllocateInfo cmdBufAllocateInfo =
                Initializers.CommandBufferAllocateInfo(
                    CmdPool,
                    VkCommandBufferLevel.Primary,
                    DrawCmdBuffers.Count);

            Util.CheckResult(vkAllocateCommandBuffers(Device, &cmdBufAllocateInfo, (VkCommandBuffer*)DrawCmdBuffers.Data));
        }

        protected virtual void SetupDepthStencil()
        {
            VkImageCreateInfo image = new VkImageCreateInfo();
            image.sType = VkStructureType.ImageCreateInfo;
            image.pNext = null;
            image.imageType = VkImageType._2d;
            image.format = DepthFormat;
            image.extent = new VkExtent3D() { width = Width, height = Height, depth = 1 };
            image.mipLevels = 1;
            image.arrayLayers = 1;
            image.samples = VkSampleCountFlagBits._1;
            image.tiling = VkImageTiling.Optimal;
            image.usage = (uint)(VkImageUsageFlagBits.DepthStencilAttachment | VkImageUsageFlagBits.TransferSrc);
            image.flags = 0;

            VkMemoryAllocateInfo mem_alloc = new VkMemoryAllocateInfo();
            mem_alloc.sType = VkStructureType.MemoryAllocateInfo;
            mem_alloc.pNext = null;
            mem_alloc.allocationSize = 0;
            mem_alloc.memoryTypeIndex = 0;

            VkImageViewCreateInfo depthStencilView = new VkImageViewCreateInfo();
            depthStencilView.sType = VkStructureType.ImageViewCreateInfo;
            depthStencilView.pNext = null;
            depthStencilView.viewType = VkImageViewType._2d;
            depthStencilView.format = DepthFormat;
            depthStencilView.flags = 0;
            depthStencilView.subresourceRange = new VkImageSubresourceRange();
            depthStencilView.subresourceRange.aspectMask = (uint)(VkImageAspectFlagBits.Depth | VkImageAspectFlagBits.Stencil);
            depthStencilView.subresourceRange.baseMipLevel = 0;
            depthStencilView.subresourceRange.levelCount = 1;
            depthStencilView.subresourceRange.baseArrayLayer = 0;
            depthStencilView.subresourceRange.layerCount = 1;

            VkMemoryRequirements memReqs;
            DepthStencil depthStencil;
            Util.CheckResult(vkCreateImage(Device, &image, null, &depthStencil.Image));
            vkGetImageMemoryRequirements(Device, depthStencil.Image, &memReqs);
            mem_alloc.allocationSize = memReqs.size;
            mem_alloc.memoryTypeIndex = VulkanDevice.getMemoryType(memReqs.memoryTypeBits, VkMemoryPropertyFlagBits.DeviceLocal);
            Util.CheckResult(vkAllocateMemory(Device, &mem_alloc, null, &depthStencil.Mem));
            Util.CheckResult(vkBindImageMemory(Device, depthStencil.Image, depthStencil.Mem, 0));

            depthStencilView.image = depthStencil.Image;
            var view = DepthStencil.View;
            Util.CheckResult(vkCreateImageView(Device, &depthStencilView, null, &view));
            depthStencil.View = view;

            DepthStencil = depthStencil;
        }

        public void RenderLoop()
        {
            destWidth = Width;
            destHeight = Height;
            while (NativeWindow.Exists)
            {
                var tStart = DateTime.Now;
                if (viewUpdated)
                {
                    viewUpdated = false;
                    viewChanged();
                }

                NativeWindow.ProcessEvents();

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
            vkDeviceWaitIdle(Device);
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
            windowTitle = Title + " - " + device;
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
            vkDeviceWaitIdle(Device);

            // Recreate swap chain
            Width = destWidth;
            Height = destHeight;
            SetupSwapChain();

            // Recreate the frame buffers

            vkDestroyImageView(Device, DepthStencil.View, null);
            vkDestroyImage(Device, DepthStencil.Image, null);
            vkFreeMemory(Device, DepthStencil.Mem, null);
            SetupDepthStencil();

            for (uint i = 0; i < Framebuffers.Count; i++)
            {
                vkDestroyFramebuffer(Device, Framebuffers[i], null);
            }
            setupFrameBuffer();

            // Command buffers need to be recreated as they may store
            // references to the recreated frame buffer
            // destroyCommandBuffers();
            CreateCommandBuffers();
            buildCommandBuffers();

            vkDeviceWaitIdle(Device);

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

        protected virtual void windowResized()
        {
        }

        protected virtual void buildCommandBuffers()
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
