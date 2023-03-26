using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
    internal static class Libc
    {
        [DllImport("libc")]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport("libc")]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport("libc")]
        public static extern int dlclose(IntPtr handle);

        [DllImport("libc")]
        public static extern string dlerror();

        public const int RTLD_NOW = 0x002;
    }
}
