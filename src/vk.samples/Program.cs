using System;
using System.Runtime.InteropServices;
using System.Text;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Vk.Samples
{
    public class Program
    {
        private static FixedUtf8String s_appName = "VulkanTestApplication";
        private static FixedUtf8String s_engineName = "VulkanEngine";
        private static FixedUtf8String s_standardValidation = "VK_LAYER_LUNARG_standard_validation";
        private static FixedUtf8String s_khrSurface = "VK_KHR_surface";
        private static FixedUtf8String s_win32Surface = "VK_KHR_win32_surface";
        private static FixedUtf8String s_debugReport = "VK_EXT_debug_report";

        public static unsafe void Main(string[] args)
        {
            byte* enabledLayersPtr = s_standardValidation.StringPtr;

            byte** enabledExtensions = stackalloc byte*[3];
            enabledExtensions[0] = s_khrSurface;
            enabledExtensions[1] = s_win32Surface;
            enabledExtensions[2] = s_debugReport;

            VkInstanceCreateInfo createInfo;
            createInfo.sType = VkStructureType.InstanceCreateInfo;
            createInfo.ppEnabledLayerNames = &enabledLayersPtr;
            createInfo.enabledLayerCount = 0;
            createInfo.ppEnabledExtensionNames = enabledExtensions;
            createInfo.enabledExtensionCount = 3;
            VkApplicationInfo applicationInfo;
            applicationInfo.sType = VkStructureType.ApplicationInfo;
            applicationInfo.engineVersion = 1;
            applicationInfo.applicationVersion = 1;
            applicationInfo.apiVersion = new Version(1, 0, 0);
            applicationInfo.pApplicationName = s_appName;
            applicationInfo.pEngineName = s_engineName;
            createInfo.pApplicationInfo = &applicationInfo;
            VkInstance instance;
            VkResult result = vkCreateInstance(&createInfo, null, &instance);
            Console.WriteLine("Result of vkCreateInstance: " + result);
        }
    }

    public unsafe class FixedUtf8String
    {
        private GCHandle _handle;

        public byte* StringPtr => (byte*)_handle.AddrOfPinnedObject().ToPointer();

        public FixedUtf8String(string s)
        {
            var text = Encoding.UTF8.GetBytes(s);
            _handle = GCHandle.Alloc(text, GCHandleType.Pinned);
        }

        public void SetText(string s)
        {
            _handle.Free();
            var text = Encoding.UTF8.GetBytes(s);
            _handle = GCHandle.Alloc(text, GCHandleType.Pinned);
        }

        public static implicit operator byte* (FixedUtf8String utf8String) => utf8String.StringPtr;
        public static implicit operator FixedUtf8String(string s) => new FixedUtf8String(s);
    }

    public struct Version
    {
        private readonly uint value;

        public Version(uint major, uint minor, uint patch)
        {
            value = major << 22 | minor << 12 | patch;
        }

        public uint Major => value >> 22;

        public uint Minor => (value >> 12) & 0x3ff;

        public uint Patch => (value >> 22) & 0xfff;

        public static implicit operator uint(Version version)
        {
            return version.value;
        }
    }
}