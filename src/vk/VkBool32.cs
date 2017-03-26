using System;

namespace Vulkan
{
    public struct VkBool32 : IEquatable<VkBool32>
    {
        public uint Value;

        public VkBool32(uint value)
        {
            Value = value;
        }

        public static VkBool32 True = new VkBool32(1);
        public static VkBool32 False = new VkBool32(0);

        public bool Equals(VkBool32 other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is VkBool32 b && Equals(b);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{(this ? "true" : "false")} ({Value})";
        }

        public static implicit operator bool(VkBool32 b) => b.Value != 0;
        public static implicit operator uint(VkBool32 b) => b.Value;
        public static implicit operator VkBool32(bool b) => b ? True : False;
        public static implicit operator VkBool32(uint value) => new VkBool32(value);

        public static bool operator ==(VkBool32 left, VkBool32 right) => left.Value == right.Value;
        public static bool operator !=(VkBool32 left, VkBool32 right) => left.Value != right.Value;
    }
}
