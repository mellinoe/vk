﻿using System;
using System.Diagnostics;
using System.IO;
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
            if (PlatformConfiguration.IsWindows)
            {
                return "vulkan-1.dll";
            }
            else if (PlatformConfiguration.IsUnix)
            {
                return "libvulkan.so.1";
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
            _libraryHandle = LoadLibrary(_libraryName);
            if (_libraryHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Could not load " + libraryName);
            }
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
            if (PlatformConfiguration.IsWindows)
            {
                return new WindowsNativeLibrary(libraryName);
            }
            else if (PlatformConfiguration.IsUnix)
            {
                return new LinuxNativeLibrary(libraryName);
            }
            else
            {
                throw new PlatformNotSupportedException("Cannot load native libraries on this platform as not supported");
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
                Libdl.dlerror();
                IntPtr handle = Libdl.dlopen(libraryName, Libdl.RTLD_NOW);
                if (handle == IntPtr.Zero && !Path.IsPathRooted(libraryName))
                {
                    string localPath = Path.Combine(PlatformConfiguration.DefaultAppDirectory, libraryName);
                    handle = Libdl.dlopen(localPath, Libdl.RTLD_NOW);
                }

                return handle;
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
