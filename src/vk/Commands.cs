using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Vulkan
{
    public static partial class VulkanNative
    {
        private static NativeLibrary s_nativeLib;

        static VulkanNative()
        {
            s_nativeLib = LoadNativeLibrary();
            LoadFunctionPointers();
        }

        private static NativeLibrary LoadNativeLibrary()
        {
            return NativeLibrary.Load(GetVulkanName());
        }

        private static string GetVulkanName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "vulkan-1.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "libvulkan.so";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        private static Exception CreateMissingFunctionException()
        {
            return new InvalidOperationException("The function does not exist or could not be loaded.");
        }
    }

    public abstract class NativeLibrary : IDisposable
    {
        private readonly string _libraryName;
        private readonly IntPtr _libraryHandle;

        public IntPtr NativeHandle => _libraryHandle;

        public NativeLibrary(string libraryName)
        {
            _libraryName = libraryName;
            _libraryHandle = LoadLibrary(libraryName);
        }

        protected abstract IntPtr LoadLibrary(string libraryName);
        protected abstract void FreeLibrary(IntPtr libraryHandle);
        protected abstract IntPtr LoadFunction(string functionName);

        public IntPtr LoadFunctionPointer(string functionName)
        {
            if (functionName == null)
            {
                throw new ArgumentNullException(nameof(functionName));
            }

            return LoadFunction(functionName);
        }

        public void Dispose()
        {
            FreeLibrary(_libraryHandle);
        }


        public static NativeLibrary Load(string libraryName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsNativeLibrary(libraryName);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxNativeLibrary(libraryName);
            }
            else
            {
                throw new PlatformNotSupportedException("Cannot load native libraries on this platform: " + RuntimeInformation.OSDescription);
            }
        }

        private class WindowsNativeLibrary : NativeLibrary
        {
            public WindowsNativeLibrary(string libraryName) : base(libraryName)
            {
            }

            protected override IntPtr LoadLibrary(string libraryName)
            {
                return Kernel32.LoadLibrary(libraryName);
            }

            protected override void FreeLibrary(IntPtr libraryHandle)
            {
                Kernel32.FreeLibrary(libraryHandle);
            }

            protected override IntPtr LoadFunction(string functionName)
            {
                Debug.WriteLine("Loading " + functionName);
                return Kernel32.GetProcAddress(NativeHandle, functionName);
            }
        }

        private class LinuxNativeLibrary : NativeLibrary
        {
            public LinuxNativeLibrary(string libraryName) : base(libraryName)
            {
            }

            protected override IntPtr LoadLibrary(string libraryName)
            {
                return Libdl.dlopen(libraryName, Libdl.RTLD_NOW);
            }

            protected override void FreeLibrary(IntPtr libraryHandle)
            {
                Libdl.dlclose(libraryHandle);
            }

            protected override IntPtr LoadFunction(string functionName)
            {
                return Libdl.dlsym(NativeHandle, functionName);
            }
        }
    }
}
