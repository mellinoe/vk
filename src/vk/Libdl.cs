using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
    internal static class Libdl
    {
        [DllImport("libdl.so")]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport("libdl.so")]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport("libdl.so")]
        public static extern int dlclose(IntPtr handle);

        public const int RTLD_NOW = 0x002;
    }
}
