using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
    public struct FunctionPointer<TFunc>
    {
        public IntPtr Pointer;

        public FunctionPointer(TFunc func)
        {
#if NET40 || NET45
            Pointer = Marshal.GetFunctionPointerForDelegate((Delegate)(object)func);
#else
            Pointer = Marshal.GetFunctionPointerForDelegate(func);
#endif
        }
    }
}
