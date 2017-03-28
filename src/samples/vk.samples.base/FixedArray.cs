namespace Vk.Samples
{
    // Fixed-size "array" types, useful for constructing inputs
    // to some Vulkan functions without allocating and pinning a real array.

    public struct FixedArray2<T> where T : struct
    {
        public T First;
        public T Second;

        public FixedArray2(T first, T second) { First = first; Second = second; }

        public uint Count => 2;
    }

    public struct FixedArray3<T> where T : struct
    {
        public T First;
        public T Second;
        public T Third;

        public FixedArray3(T first, T second, T third) { First = first; Second = second; Third = third; }

        public uint Count => 3;
    }

    public struct FixedArray4<T> where T : struct
    {
        public T First;
        public T Second;
        public T Third;
        public T Fourth;

        public FixedArray4(T first, T second, T third, T fourth) { First = first; Second = second; Third = third; Fourth = fourth; }

        public uint Count => 4;
    }

    public struct FixedArray6<T> where T : struct
    {
        public T First;
        public T Second;
        public T Third;
        public T Fourth;
        public T Fifth;
        public T Sixth;

        public FixedArray6(T first, T second, T third, T fourth, T fifth, T sixth) { First = first; Second = second; Third = third; Fourth = fourth; Fifth = fifth; Sixth = sixth; }

        public uint Count => 6;
    }
}
