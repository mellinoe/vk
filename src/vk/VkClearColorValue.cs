using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vulkan
{
    /// <summary>
    /// Structure specifying a clear color value.
    /// </summary>
    /// <summary>
    /// Structure specifying a clear color value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VkClearColorValue
    {
        [FieldOffset(0)] public VkColor4 Float4;
        [FieldOffset(0)] public VkColorI4 Int4;
        [FieldOffset(0)] public VkColorU4 UInt4;

        public VkClearColorValue(float r, float g, float b, float a = 1.0f) : this()
        {
            Float4 = new VkColor4(r, g, b, a);
        }

        public VkClearColorValue(VkColor4 value) : this()
        {
            Float4 = value;
        }

        public VkClearColorValue(VkColorI4 value) : this()
        {
            Int4 = value;
        }

        public VkClearColorValue(VkColorU4 value) : this()
        {
            UInt4 = value;
        }
    }
}
