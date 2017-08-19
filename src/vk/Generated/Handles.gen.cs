// This file is generated.

using System;
using System.Diagnostics;

namespace Vulkan
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkInstance : IEquatable<VkInstance>
    {
        public readonly IntPtr Handle;
        public VkInstance(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkInstance Null => new VkInstance(IntPtr.Zero);
        public static implicit operator VkInstance(IntPtr handle) => new VkInstance(handle);
        public static bool operator ==(VkInstance left, VkInstance right) => left.Handle == right.Handle;
        public static bool operator !=(VkInstance left, VkInstance right) => left.Handle != right.Handle;
        public static bool operator ==(VkInstance left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkInstance left, IntPtr right) => left.Handle != right;
        public bool Equals(VkInstance h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkInstance h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkInstance [{0}]", Handle);
    }

    ///<summary>A dispatchable handle owned by a VkInstance.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkPhysicalDevice : IEquatable<VkPhysicalDevice>
    {
        public readonly IntPtr Handle;
        public VkPhysicalDevice(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPhysicalDevice Null => new VkPhysicalDevice(IntPtr.Zero);
        public static implicit operator VkPhysicalDevice(IntPtr handle) => new VkPhysicalDevice(handle);
        public static bool operator ==(VkPhysicalDevice left, VkPhysicalDevice right) => left.Handle == right.Handle;
        public static bool operator !=(VkPhysicalDevice left, VkPhysicalDevice right) => left.Handle != right.Handle;
        public static bool operator ==(VkPhysicalDevice left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPhysicalDevice left, IntPtr right) => left.Handle != right;
        public bool Equals(VkPhysicalDevice h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkPhysicalDevice h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkPhysicalDevice [{0}]", Handle);
    }

    ///<summary>A dispatchable handle owned by a VkPhysicalDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDevice : IEquatable<VkDevice>
    {
        public readonly IntPtr Handle;
        public VkDevice(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDevice Null => new VkDevice(IntPtr.Zero);
        public static implicit operator VkDevice(IntPtr handle) => new VkDevice(handle);
        public static bool operator ==(VkDevice left, VkDevice right) => left.Handle == right.Handle;
        public static bool operator !=(VkDevice left, VkDevice right) => left.Handle != right.Handle;
        public static bool operator ==(VkDevice left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDevice left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDevice h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDevice h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDevice [{0}]", Handle);
    }

    ///<summary>A dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkQueue : IEquatable<VkQueue>
    {
        public readonly IntPtr Handle;
        public VkQueue(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkQueue Null => new VkQueue(IntPtr.Zero);
        public static implicit operator VkQueue(IntPtr handle) => new VkQueue(handle);
        public static bool operator ==(VkQueue left, VkQueue right) => left.Handle == right.Handle;
        public static bool operator !=(VkQueue left, VkQueue right) => left.Handle != right.Handle;
        public static bool operator ==(VkQueue left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkQueue left, IntPtr right) => left.Handle != right;
        public bool Equals(VkQueue h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkQueue h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkQueue [{0}]", Handle);
    }

    ///<summary>A dispatchable handle owned by a VkCommandPool.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkCommandBuffer : IEquatable<VkCommandBuffer>
    {
        public readonly IntPtr Handle;
        public VkCommandBuffer(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkCommandBuffer Null => new VkCommandBuffer(IntPtr.Zero);
        public static implicit operator VkCommandBuffer(IntPtr handle) => new VkCommandBuffer(handle);
        public static bool operator ==(VkCommandBuffer left, VkCommandBuffer right) => left.Handle == right.Handle;
        public static bool operator !=(VkCommandBuffer left, VkCommandBuffer right) => left.Handle != right.Handle;
        public static bool operator ==(VkCommandBuffer left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkCommandBuffer left, IntPtr right) => left.Handle != right;
        public bool Equals(VkCommandBuffer h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkCommandBuffer h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkCommandBuffer [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDeviceMemory : IEquatable<VkDeviceMemory>
    {
        public readonly IntPtr Handle;
        public VkDeviceMemory(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDeviceMemory Null => new VkDeviceMemory(IntPtr.Zero);
        public static implicit operator VkDeviceMemory(IntPtr handle) => new VkDeviceMemory(handle);
        public static bool operator ==(VkDeviceMemory left, VkDeviceMemory right) => left.Handle == right.Handle;
        public static bool operator !=(VkDeviceMemory left, VkDeviceMemory right) => left.Handle != right.Handle;
        public static bool operator ==(VkDeviceMemory left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDeviceMemory left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDeviceMemory h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDeviceMemory h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDeviceMemory [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkCommandPool : IEquatable<VkCommandPool>
    {
        public readonly IntPtr Handle;
        public VkCommandPool(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkCommandPool Null => new VkCommandPool(IntPtr.Zero);
        public static implicit operator VkCommandPool(IntPtr handle) => new VkCommandPool(handle);
        public static bool operator ==(VkCommandPool left, VkCommandPool right) => left.Handle == right.Handle;
        public static bool operator !=(VkCommandPool left, VkCommandPool right) => left.Handle != right.Handle;
        public static bool operator ==(VkCommandPool left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkCommandPool left, IntPtr right) => left.Handle != right;
        public bool Equals(VkCommandPool h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkCommandPool h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkCommandPool [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkBuffer : IEquatable<VkBuffer>
    {
        public readonly IntPtr Handle;
        public VkBuffer(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkBuffer Null => new VkBuffer(IntPtr.Zero);
        public static implicit operator VkBuffer(IntPtr handle) => new VkBuffer(handle);
        public static bool operator ==(VkBuffer left, VkBuffer right) => left.Handle == right.Handle;
        public static bool operator !=(VkBuffer left, VkBuffer right) => left.Handle != right.Handle;
        public static bool operator ==(VkBuffer left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkBuffer left, IntPtr right) => left.Handle != right;
        public bool Equals(VkBuffer h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkBuffer h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkBuffer [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkBufferView : IEquatable<VkBufferView>
    {
        public readonly IntPtr Handle;
        public VkBufferView(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkBufferView Null => new VkBufferView(IntPtr.Zero);
        public static implicit operator VkBufferView(IntPtr handle) => new VkBufferView(handle);
        public static bool operator ==(VkBufferView left, VkBufferView right) => left.Handle == right.Handle;
        public static bool operator !=(VkBufferView left, VkBufferView right) => left.Handle != right.Handle;
        public static bool operator ==(VkBufferView left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkBufferView left, IntPtr right) => left.Handle != right;
        public bool Equals(VkBufferView h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkBufferView h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkBufferView [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkImage : IEquatable<VkImage>
    {
        public readonly IntPtr Handle;
        public VkImage(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkImage Null => new VkImage(IntPtr.Zero);
        public static implicit operator VkImage(IntPtr handle) => new VkImage(handle);
        public static bool operator ==(VkImage left, VkImage right) => left.Handle == right.Handle;
        public static bool operator !=(VkImage left, VkImage right) => left.Handle != right.Handle;
        public static bool operator ==(VkImage left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkImage left, IntPtr right) => left.Handle != right;
        public bool Equals(VkImage h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkImage h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkImage [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkImageView : IEquatable<VkImageView>
    {
        public readonly IntPtr Handle;
        public VkImageView(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkImageView Null => new VkImageView(IntPtr.Zero);
        public static implicit operator VkImageView(IntPtr handle) => new VkImageView(handle);
        public static bool operator ==(VkImageView left, VkImageView right) => left.Handle == right.Handle;
        public static bool operator !=(VkImageView left, VkImageView right) => left.Handle != right.Handle;
        public static bool operator ==(VkImageView left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkImageView left, IntPtr right) => left.Handle != right;
        public bool Equals(VkImageView h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkImageView h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkImageView [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkShaderModule : IEquatable<VkShaderModule>
    {
        public readonly IntPtr Handle;
        public VkShaderModule(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkShaderModule Null => new VkShaderModule(IntPtr.Zero);
        public static implicit operator VkShaderModule(IntPtr handle) => new VkShaderModule(handle);
        public static bool operator ==(VkShaderModule left, VkShaderModule right) => left.Handle == right.Handle;
        public static bool operator !=(VkShaderModule left, VkShaderModule right) => left.Handle != right.Handle;
        public static bool operator ==(VkShaderModule left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkShaderModule left, IntPtr right) => left.Handle != right;
        public bool Equals(VkShaderModule h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkShaderModule h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkShaderModule [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkPipeline : IEquatable<VkPipeline>
    {
        public readonly IntPtr Handle;
        public VkPipeline(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPipeline Null => new VkPipeline(IntPtr.Zero);
        public static implicit operator VkPipeline(IntPtr handle) => new VkPipeline(handle);
        public static bool operator ==(VkPipeline left, VkPipeline right) => left.Handle == right.Handle;
        public static bool operator !=(VkPipeline left, VkPipeline right) => left.Handle != right.Handle;
        public static bool operator ==(VkPipeline left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPipeline left, IntPtr right) => left.Handle != right;
        public bool Equals(VkPipeline h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkPipeline h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkPipeline [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkPipelineLayout : IEquatable<VkPipelineLayout>
    {
        public readonly IntPtr Handle;
        public VkPipelineLayout(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPipelineLayout Null => new VkPipelineLayout(IntPtr.Zero);
        public static implicit operator VkPipelineLayout(IntPtr handle) => new VkPipelineLayout(handle);
        public static bool operator ==(VkPipelineLayout left, VkPipelineLayout right) => left.Handle == right.Handle;
        public static bool operator !=(VkPipelineLayout left, VkPipelineLayout right) => left.Handle != right.Handle;
        public static bool operator ==(VkPipelineLayout left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPipelineLayout left, IntPtr right) => left.Handle != right;
        public bool Equals(VkPipelineLayout h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkPipelineLayout h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkPipelineLayout [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkSampler : IEquatable<VkSampler>
    {
        public readonly IntPtr Handle;
        public VkSampler(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSampler Null => new VkSampler(IntPtr.Zero);
        public static implicit operator VkSampler(IntPtr handle) => new VkSampler(handle);
        public static bool operator ==(VkSampler left, VkSampler right) => left.Handle == right.Handle;
        public static bool operator !=(VkSampler left, VkSampler right) => left.Handle != right.Handle;
        public static bool operator ==(VkSampler left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSampler left, IntPtr right) => left.Handle != right;
        public bool Equals(VkSampler h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkSampler h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkSampler [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDescriptorPool.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDescriptorSet : IEquatable<VkDescriptorSet>
    {
        public readonly IntPtr Handle;
        public VkDescriptorSet(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDescriptorSet Null => new VkDescriptorSet(IntPtr.Zero);
        public static implicit operator VkDescriptorSet(IntPtr handle) => new VkDescriptorSet(handle);
        public static bool operator ==(VkDescriptorSet left, VkDescriptorSet right) => left.Handle == right.Handle;
        public static bool operator !=(VkDescriptorSet left, VkDescriptorSet right) => left.Handle != right.Handle;
        public static bool operator ==(VkDescriptorSet left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDescriptorSet left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDescriptorSet h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDescriptorSet h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDescriptorSet [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDescriptorSetLayout : IEquatable<VkDescriptorSetLayout>
    {
        public readonly IntPtr Handle;
        public VkDescriptorSetLayout(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDescriptorSetLayout Null => new VkDescriptorSetLayout(IntPtr.Zero);
        public static implicit operator VkDescriptorSetLayout(IntPtr handle) => new VkDescriptorSetLayout(handle);
        public static bool operator ==(VkDescriptorSetLayout left, VkDescriptorSetLayout right) => left.Handle == right.Handle;
        public static bool operator !=(VkDescriptorSetLayout left, VkDescriptorSetLayout right) => left.Handle != right.Handle;
        public static bool operator ==(VkDescriptorSetLayout left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDescriptorSetLayout left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDescriptorSetLayout h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDescriptorSetLayout h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDescriptorSetLayout [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDescriptorPool : IEquatable<VkDescriptorPool>
    {
        public readonly IntPtr Handle;
        public VkDescriptorPool(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDescriptorPool Null => new VkDescriptorPool(IntPtr.Zero);
        public static implicit operator VkDescriptorPool(IntPtr handle) => new VkDescriptorPool(handle);
        public static bool operator ==(VkDescriptorPool left, VkDescriptorPool right) => left.Handle == right.Handle;
        public static bool operator !=(VkDescriptorPool left, VkDescriptorPool right) => left.Handle != right.Handle;
        public static bool operator ==(VkDescriptorPool left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDescriptorPool left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDescriptorPool h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDescriptorPool h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDescriptorPool [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkFence : IEquatable<VkFence>
    {
        public readonly IntPtr Handle;
        public VkFence(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkFence Null => new VkFence(IntPtr.Zero);
        public static implicit operator VkFence(IntPtr handle) => new VkFence(handle);
        public static bool operator ==(VkFence left, VkFence right) => left.Handle == right.Handle;
        public static bool operator !=(VkFence left, VkFence right) => left.Handle != right.Handle;
        public static bool operator ==(VkFence left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkFence left, IntPtr right) => left.Handle != right;
        public bool Equals(VkFence h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkFence h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkFence [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkSemaphore : IEquatable<VkSemaphore>
    {
        public readonly IntPtr Handle;
        public VkSemaphore(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSemaphore Null => new VkSemaphore(IntPtr.Zero);
        public static implicit operator VkSemaphore(IntPtr handle) => new VkSemaphore(handle);
        public static bool operator ==(VkSemaphore left, VkSemaphore right) => left.Handle == right.Handle;
        public static bool operator !=(VkSemaphore left, VkSemaphore right) => left.Handle != right.Handle;
        public static bool operator ==(VkSemaphore left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSemaphore left, IntPtr right) => left.Handle != right;
        public bool Equals(VkSemaphore h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkSemaphore h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkSemaphore [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkEvent : IEquatable<VkEvent>
    {
        public readonly IntPtr Handle;
        public VkEvent(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkEvent Null => new VkEvent(IntPtr.Zero);
        public static implicit operator VkEvent(IntPtr handle) => new VkEvent(handle);
        public static bool operator ==(VkEvent left, VkEvent right) => left.Handle == right.Handle;
        public static bool operator !=(VkEvent left, VkEvent right) => left.Handle != right.Handle;
        public static bool operator ==(VkEvent left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkEvent left, IntPtr right) => left.Handle != right;
        public bool Equals(VkEvent h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkEvent h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkEvent [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkQueryPool : IEquatable<VkQueryPool>
    {
        public readonly IntPtr Handle;
        public VkQueryPool(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkQueryPool Null => new VkQueryPool(IntPtr.Zero);
        public static implicit operator VkQueryPool(IntPtr handle) => new VkQueryPool(handle);
        public static bool operator ==(VkQueryPool left, VkQueryPool right) => left.Handle == right.Handle;
        public static bool operator !=(VkQueryPool left, VkQueryPool right) => left.Handle != right.Handle;
        public static bool operator ==(VkQueryPool left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkQueryPool left, IntPtr right) => left.Handle != right;
        public bool Equals(VkQueryPool h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkQueryPool h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkQueryPool [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkFramebuffer : IEquatable<VkFramebuffer>
    {
        public readonly IntPtr Handle;
        public VkFramebuffer(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkFramebuffer Null => new VkFramebuffer(IntPtr.Zero);
        public static implicit operator VkFramebuffer(IntPtr handle) => new VkFramebuffer(handle);
        public static bool operator ==(VkFramebuffer left, VkFramebuffer right) => left.Handle == right.Handle;
        public static bool operator !=(VkFramebuffer left, VkFramebuffer right) => left.Handle != right.Handle;
        public static bool operator ==(VkFramebuffer left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkFramebuffer left, IntPtr right) => left.Handle != right;
        public bool Equals(VkFramebuffer h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkFramebuffer h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkFramebuffer [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkRenderPass : IEquatable<VkRenderPass>
    {
        public readonly IntPtr Handle;
        public VkRenderPass(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkRenderPass Null => new VkRenderPass(IntPtr.Zero);
        public static implicit operator VkRenderPass(IntPtr handle) => new VkRenderPass(handle);
        public static bool operator ==(VkRenderPass left, VkRenderPass right) => left.Handle == right.Handle;
        public static bool operator !=(VkRenderPass left, VkRenderPass right) => left.Handle != right.Handle;
        public static bool operator ==(VkRenderPass left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkRenderPass left, IntPtr right) => left.Handle != right;
        public bool Equals(VkRenderPass h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkRenderPass h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkRenderPass [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkPipelineCache : IEquatable<VkPipelineCache>
    {
        public readonly IntPtr Handle;
        public VkPipelineCache(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkPipelineCache Null => new VkPipelineCache(IntPtr.Zero);
        public static implicit operator VkPipelineCache(IntPtr handle) => new VkPipelineCache(handle);
        public static bool operator ==(VkPipelineCache left, VkPipelineCache right) => left.Handle == right.Handle;
        public static bool operator !=(VkPipelineCache left, VkPipelineCache right) => left.Handle != right.Handle;
        public static bool operator ==(VkPipelineCache left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkPipelineCache left, IntPtr right) => left.Handle != right;
        public bool Equals(VkPipelineCache h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkPipelineCache h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkPipelineCache [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkObjectTableNVX : IEquatable<VkObjectTableNVX>
    {
        public readonly IntPtr Handle;
        public VkObjectTableNVX(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkObjectTableNVX Null => new VkObjectTableNVX(IntPtr.Zero);
        public static implicit operator VkObjectTableNVX(IntPtr handle) => new VkObjectTableNVX(handle);
        public static bool operator ==(VkObjectTableNVX left, VkObjectTableNVX right) => left.Handle == right.Handle;
        public static bool operator !=(VkObjectTableNVX left, VkObjectTableNVX right) => left.Handle != right.Handle;
        public static bool operator ==(VkObjectTableNVX left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkObjectTableNVX left, IntPtr right) => left.Handle != right;
        public bool Equals(VkObjectTableNVX h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkObjectTableNVX h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkObjectTableNVX [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkIndirectCommandsLayoutNVX : IEquatable<VkIndirectCommandsLayoutNVX>
    {
        public readonly IntPtr Handle;
        public VkIndirectCommandsLayoutNVX(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkIndirectCommandsLayoutNVX Null => new VkIndirectCommandsLayoutNVX(IntPtr.Zero);
        public static implicit operator VkIndirectCommandsLayoutNVX(IntPtr handle) => new VkIndirectCommandsLayoutNVX(handle);
        public static bool operator ==(VkIndirectCommandsLayoutNVX left, VkIndirectCommandsLayoutNVX right) => left.Handle == right.Handle;
        public static bool operator !=(VkIndirectCommandsLayoutNVX left, VkIndirectCommandsLayoutNVX right) => left.Handle != right.Handle;
        public static bool operator ==(VkIndirectCommandsLayoutNVX left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkIndirectCommandsLayoutNVX left, IntPtr right) => left.Handle != right;
        public bool Equals(VkIndirectCommandsLayoutNVX h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkIndirectCommandsLayoutNVX h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkIndirectCommandsLayoutNVX [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkDevice.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDescriptorUpdateTemplateKHR : IEquatable<VkDescriptorUpdateTemplateKHR>
    {
        public readonly IntPtr Handle;
        public VkDescriptorUpdateTemplateKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDescriptorUpdateTemplateKHR Null => new VkDescriptorUpdateTemplateKHR(IntPtr.Zero);
        public static implicit operator VkDescriptorUpdateTemplateKHR(IntPtr handle) => new VkDescriptorUpdateTemplateKHR(handle);
        public static bool operator ==(VkDescriptorUpdateTemplateKHR left, VkDescriptorUpdateTemplateKHR right) => left.Handle == right.Handle;
        public static bool operator !=(VkDescriptorUpdateTemplateKHR left, VkDescriptorUpdateTemplateKHR right) => left.Handle != right.Handle;
        public static bool operator ==(VkDescriptorUpdateTemplateKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDescriptorUpdateTemplateKHR left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDescriptorUpdateTemplateKHR h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDescriptorUpdateTemplateKHR h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDescriptorUpdateTemplateKHR [{0}]", Handle);
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDisplayKHR : IEquatable<VkDisplayKHR>
    {
        public readonly IntPtr Handle;
        public VkDisplayKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDisplayKHR Null => new VkDisplayKHR(IntPtr.Zero);
        public static implicit operator VkDisplayKHR(IntPtr handle) => new VkDisplayKHR(handle);
        public static bool operator ==(VkDisplayKHR left, VkDisplayKHR right) => left.Handle == right.Handle;
        public static bool operator !=(VkDisplayKHR left, VkDisplayKHR right) => left.Handle != right.Handle;
        public static bool operator ==(VkDisplayKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDisplayKHR left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDisplayKHR h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDisplayKHR h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDisplayKHR [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkPhysicalDevice,VkDisplayKHR.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDisplayModeKHR : IEquatable<VkDisplayModeKHR>
    {
        public readonly IntPtr Handle;
        public VkDisplayModeKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDisplayModeKHR Null => new VkDisplayModeKHR(IntPtr.Zero);
        public static implicit operator VkDisplayModeKHR(IntPtr handle) => new VkDisplayModeKHR(handle);
        public static bool operator ==(VkDisplayModeKHR left, VkDisplayModeKHR right) => left.Handle == right.Handle;
        public static bool operator !=(VkDisplayModeKHR left, VkDisplayModeKHR right) => left.Handle != right.Handle;
        public static bool operator ==(VkDisplayModeKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDisplayModeKHR left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDisplayModeKHR h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDisplayModeKHR h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDisplayModeKHR [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkInstance.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkSurfaceKHR : IEquatable<VkSurfaceKHR>
    {
        public readonly IntPtr Handle;
        public VkSurfaceKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSurfaceKHR Null => new VkSurfaceKHR(IntPtr.Zero);
        public static implicit operator VkSurfaceKHR(IntPtr handle) => new VkSurfaceKHR(handle);
        public static bool operator ==(VkSurfaceKHR left, VkSurfaceKHR right) => left.Handle == right.Handle;
        public static bool operator !=(VkSurfaceKHR left, VkSurfaceKHR right) => left.Handle != right.Handle;
        public static bool operator ==(VkSurfaceKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSurfaceKHR left, IntPtr right) => left.Handle != right;
        public bool Equals(VkSurfaceKHR h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkSurfaceKHR h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkSurfaceKHR [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkSurfaceKHR.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkSwapchainKHR : IEquatable<VkSwapchainKHR>
    {
        public readonly IntPtr Handle;
        public VkSwapchainKHR(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkSwapchainKHR Null => new VkSwapchainKHR(IntPtr.Zero);
        public static implicit operator VkSwapchainKHR(IntPtr handle) => new VkSwapchainKHR(handle);
        public static bool operator ==(VkSwapchainKHR left, VkSwapchainKHR right) => left.Handle == right.Handle;
        public static bool operator !=(VkSwapchainKHR left, VkSwapchainKHR right) => left.Handle != right.Handle;
        public static bool operator ==(VkSwapchainKHR left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkSwapchainKHR left, IntPtr right) => left.Handle != right;
        public bool Equals(VkSwapchainKHR h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkSwapchainKHR h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkSwapchainKHR [{0}]", Handle);
    }

    ///<summary>A non-dispatchable handle owned by a VkInstance.</summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct VkDebugReportCallbackEXT : IEquatable<VkDebugReportCallbackEXT>
    {
        public readonly IntPtr Handle;
        public VkDebugReportCallbackEXT(IntPtr existingHandle) { Handle = existingHandle; }
        public static VkDebugReportCallbackEXT Null => new VkDebugReportCallbackEXT(IntPtr.Zero);
        public static implicit operator VkDebugReportCallbackEXT(IntPtr handle) => new VkDebugReportCallbackEXT(handle);
        public static bool operator ==(VkDebugReportCallbackEXT left, VkDebugReportCallbackEXT right) => left.Handle == right.Handle;
        public static bool operator !=(VkDebugReportCallbackEXT left, VkDebugReportCallbackEXT right) => left.Handle != right.Handle;
        public static bool operator ==(VkDebugReportCallbackEXT left, IntPtr right) => left.Handle == right;
        public static bool operator !=(VkDebugReportCallbackEXT left, IntPtr right) => left.Handle != right;
        public bool Equals(VkDebugReportCallbackEXT h) => Handle.Equals(h.Handle);
        public override bool Equals(object o) => o is VkDebugReportCallbackEXT h && Equals(h);
        public override int GetHashCode() => Handle.GetHashCode();
        private string DebuggerDisplay => string.Format("VkDebugReportCallbackEXT [{0}]", Handle);
    }
}
