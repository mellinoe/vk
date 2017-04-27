// This file is generated.

using System;

namespace Vulkan
{
    public partial struct VkInstance
    {
        public readonly IntPtr Handle;
        public VkInstance(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkInstance Null => new VkInstance(IntPtr.Zero);
        public static implicit operator VkInstance(IntPtr handle) => new VkInstance(handle);
        public static bool operator ==(VkInstance left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkInstance left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A dispatchable handle owned by a VkInstance.</summary>
    public partial struct VkPhysicalDevice
    {
        public readonly IntPtr Handle;
        public VkPhysicalDevice(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPhysicalDevice Null => new VkPhysicalDevice(IntPtr.Zero);
        public static implicit operator VkPhysicalDevice(IntPtr handle) => new VkPhysicalDevice(handle);
        public static bool operator ==(VkPhysicalDevice left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPhysicalDevice left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A dispatchable handle owned by a VkPhysicalDevice.</summary>
    public partial struct VkDevice
    {
        public readonly IntPtr Handle;
        public VkDevice(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDevice Null => new VkDevice(IntPtr.Zero);
        public static implicit operator VkDevice(IntPtr handle) => new VkDevice(handle);
        public static bool operator ==(VkDevice left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDevice left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkQueue
    {
        public readonly IntPtr Handle;
        public VkQueue(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkQueue Null => new VkQueue(IntPtr.Zero);
        public static implicit operator VkQueue(IntPtr handle) => new VkQueue(handle);
        public static bool operator ==(VkQueue left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkQueue left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A dispatchable handle owned by a VkCommandPool.</summary>
    public partial struct VkCommandBuffer
    {
        public readonly IntPtr Handle;
        public VkCommandBuffer(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkCommandBuffer Null => new VkCommandBuffer(IntPtr.Zero);
        public static implicit operator VkCommandBuffer(IntPtr handle) => new VkCommandBuffer(handle);
        public static bool operator ==(VkCommandBuffer left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkCommandBuffer left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkDeviceMemory
    {
        public readonly IntPtr Handle;
        public VkDeviceMemory(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDeviceMemory Null => new VkDeviceMemory(IntPtr.Zero);
        public static implicit operator VkDeviceMemory(IntPtr handle) => new VkDeviceMemory(handle);
        public static bool operator ==(VkDeviceMemory left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDeviceMemory left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkCommandPool
    {
        public readonly IntPtr Handle;
        public VkCommandPool(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkCommandPool Null => new VkCommandPool(IntPtr.Zero);
        public static implicit operator VkCommandPool(IntPtr handle) => new VkCommandPool(handle);
        public static bool operator ==(VkCommandPool left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkCommandPool left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkBuffer
    {
        public readonly IntPtr Handle;
        public VkBuffer(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkBuffer Null => new VkBuffer(IntPtr.Zero);
        public static implicit operator VkBuffer(IntPtr handle) => new VkBuffer(handle);
        public static bool operator ==(VkBuffer left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkBuffer left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkBufferView
    {
        public readonly IntPtr Handle;
        public VkBufferView(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkBufferView Null => new VkBufferView(IntPtr.Zero);
        public static implicit operator VkBufferView(IntPtr handle) => new VkBufferView(handle);
        public static bool operator ==(VkBufferView left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkBufferView left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkImage
    {
        public readonly IntPtr Handle;
        public VkImage(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkImage Null => new VkImage(IntPtr.Zero);
        public static implicit operator VkImage(IntPtr handle) => new VkImage(handle);
        public static bool operator ==(VkImage left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkImage left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkImageView
    {
        public readonly IntPtr Handle;
        public VkImageView(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkImageView Null => new VkImageView(IntPtr.Zero);
        public static implicit operator VkImageView(IntPtr handle) => new VkImageView(handle);
        public static bool operator ==(VkImageView left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkImageView left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkShaderModule
    {
        public readonly IntPtr Handle;
        public VkShaderModule(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkShaderModule Null => new VkShaderModule(IntPtr.Zero);
        public static implicit operator VkShaderModule(IntPtr handle) => new VkShaderModule(handle);
        public static bool operator ==(VkShaderModule left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkShaderModule left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkPipeline
    {
        public readonly IntPtr Handle;
        public VkPipeline(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPipeline Null => new VkPipeline(IntPtr.Zero);
        public static implicit operator VkPipeline(IntPtr handle) => new VkPipeline(handle);
        public static bool operator ==(VkPipeline left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPipeline left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkPipelineLayout
    {
        public readonly IntPtr Handle;
        public VkPipelineLayout(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPipelineLayout Null => new VkPipelineLayout(IntPtr.Zero);
        public static implicit operator VkPipelineLayout(IntPtr handle) => new VkPipelineLayout(handle);
        public static bool operator ==(VkPipelineLayout left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPipelineLayout left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkSampler
    {
        public readonly IntPtr Handle;
        public VkSampler(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSampler Null => new VkSampler(IntPtr.Zero);
        public static implicit operator VkSampler(IntPtr handle) => new VkSampler(handle);
        public static bool operator ==(VkSampler left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSampler left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDescriptorPool.</summary>
    public partial struct VkDescriptorSet
    {
        public readonly IntPtr Handle;
        public VkDescriptorSet(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDescriptorSet Null => new VkDescriptorSet(IntPtr.Zero);
        public static implicit operator VkDescriptorSet(IntPtr handle) => new VkDescriptorSet(handle);
        public static bool operator ==(VkDescriptorSet left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDescriptorSet left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkDescriptorSetLayout
    {
        public readonly IntPtr Handle;
        public VkDescriptorSetLayout(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDescriptorSetLayout Null => new VkDescriptorSetLayout(IntPtr.Zero);
        public static implicit operator VkDescriptorSetLayout(IntPtr handle) => new VkDescriptorSetLayout(handle);
        public static bool operator ==(VkDescriptorSetLayout left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDescriptorSetLayout left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkDescriptorPool
    {
        public readonly IntPtr Handle;
        public VkDescriptorPool(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDescriptorPool Null => new VkDescriptorPool(IntPtr.Zero);
        public static implicit operator VkDescriptorPool(IntPtr handle) => new VkDescriptorPool(handle);
        public static bool operator ==(VkDescriptorPool left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDescriptorPool left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkFence
    {
        public readonly IntPtr Handle;
        public VkFence(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkFence Null => new VkFence(IntPtr.Zero);
        public static implicit operator VkFence(IntPtr handle) => new VkFence(handle);
        public static bool operator ==(VkFence left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkFence left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkSemaphore
    {
        public readonly IntPtr Handle;
        public VkSemaphore(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSemaphore Null => new VkSemaphore(IntPtr.Zero);
        public static implicit operator VkSemaphore(IntPtr handle) => new VkSemaphore(handle);
        public static bool operator ==(VkSemaphore left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSemaphore left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkEvent
    {
        public readonly IntPtr Handle;
        public VkEvent(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkEvent Null => new VkEvent(IntPtr.Zero);
        public static implicit operator VkEvent(IntPtr handle) => new VkEvent(handle);
        public static bool operator ==(VkEvent left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkEvent left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkQueryPool
    {
        public readonly IntPtr Handle;
        public VkQueryPool(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkQueryPool Null => new VkQueryPool(IntPtr.Zero);
        public static implicit operator VkQueryPool(IntPtr handle) => new VkQueryPool(handle);
        public static bool operator ==(VkQueryPool left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkQueryPool left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkFramebuffer
    {
        public readonly IntPtr Handle;
        public VkFramebuffer(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkFramebuffer Null => new VkFramebuffer(IntPtr.Zero);
        public static implicit operator VkFramebuffer(IntPtr handle) => new VkFramebuffer(handle);
        public static bool operator ==(VkFramebuffer left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkFramebuffer left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkRenderPass
    {
        public readonly IntPtr Handle;
        public VkRenderPass(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkRenderPass Null => new VkRenderPass(IntPtr.Zero);
        public static implicit operator VkRenderPass(IntPtr handle) => new VkRenderPass(handle);
        public static bool operator ==(VkRenderPass left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkRenderPass left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkPipelineCache
    {
        public readonly IntPtr Handle;
        public VkPipelineCache(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPipelineCache Null => new VkPipelineCache(IntPtr.Zero);
        public static implicit operator VkPipelineCache(IntPtr handle) => new VkPipelineCache(handle);
        public static bool operator ==(VkPipelineCache left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPipelineCache left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkObjectTableNVX
    {
        public readonly IntPtr Handle;
        public VkObjectTableNVX(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkObjectTableNVX Null => new VkObjectTableNVX(IntPtr.Zero);
        public static implicit operator VkObjectTableNVX(IntPtr handle) => new VkObjectTableNVX(handle);
        public static bool operator ==(VkObjectTableNVX left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkObjectTableNVX left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    public partial struct VkIndirectCommandsLayoutNVX
    {
        public readonly IntPtr Handle;
        public VkIndirectCommandsLayoutNVX(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkIndirectCommandsLayoutNVX Null => new VkIndirectCommandsLayoutNVX(IntPtr.Zero);
        public static implicit operator VkIndirectCommandsLayoutNVX(IntPtr handle) => new VkIndirectCommandsLayoutNVX(handle);
        public static bool operator ==(VkIndirectCommandsLayoutNVX left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkIndirectCommandsLayoutNVX left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    public partial struct VkDisplayKHR
    {
        public readonly IntPtr Handle;
        public VkDisplayKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDisplayKHR Null => new VkDisplayKHR(IntPtr.Zero);
        public static implicit operator VkDisplayKHR(IntPtr handle) => new VkDisplayKHR(handle);
        public static bool operator ==(VkDisplayKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDisplayKHR left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkPhysicalDevice,VkDisplayKHR.</summary>
    public partial struct VkDisplayModeKHR
    {
        public readonly IntPtr Handle;
        public VkDisplayModeKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDisplayModeKHR Null => new VkDisplayModeKHR(IntPtr.Zero);
        public static implicit operator VkDisplayModeKHR(IntPtr handle) => new VkDisplayModeKHR(handle);
        public static bool operator ==(VkDisplayModeKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDisplayModeKHR left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkInstance.</summary>
    public partial struct VkSurfaceKHR
    {
        public readonly IntPtr Handle;
        public VkSurfaceKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSurfaceKHR Null => new VkSurfaceKHR(IntPtr.Zero);
        public static implicit operator VkSurfaceKHR(IntPtr handle) => new VkSurfaceKHR(handle);
        public static bool operator ==(VkSurfaceKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSurfaceKHR left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkSurfaceKHR.</summary>
    public partial struct VkSwapchainKHR
    {
        public readonly IntPtr Handle;
        public VkSwapchainKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSwapchainKHR Null => new VkSwapchainKHR(IntPtr.Zero);
        public static implicit operator VkSwapchainKHR(IntPtr handle) => new VkSwapchainKHR(handle);
        public static bool operator ==(VkSwapchainKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSwapchainKHR left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }

    ///<summary>A non-dispatchable handle owned by a VkInstance.</summary>
    public partial struct VkDebugReportCallbackEXT
    {
        public readonly IntPtr Handle;
        public VkDebugReportCallbackEXT(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDebugReportCallbackEXT Null => new VkDebugReportCallbackEXT(IntPtr.Zero);
        public static implicit operator VkDebugReportCallbackEXT(IntPtr handle) => new VkDebugReportCallbackEXT(handle);
        public static bool operator ==(VkDebugReportCallbackEXT left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDebugReportCallbackEXT left, IntPtr right) => left.Handle != right;
        public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);
        public override int GetHashCode() => Handle.GetHashCode();
    }
}
