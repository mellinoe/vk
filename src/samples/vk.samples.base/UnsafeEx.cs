using System;
using System.Runtime.CompilerServices;

namespace Vk.Samples
{
    public static unsafe class UnsafeEx
    {
        public static IntPtr AsIntPtr<T>(ref T obj)
        {
            return new IntPtr(Unsafe.AsPointer(ref obj));
        }
    }
}
