namespace Vulkan
{
    public struct VkBool32
    {
        public uint Value;

        public VkBool32(uint value)
        {
            Value = value;
        }

        public static VkBool32 True = new VkBool32(1);
        public static VkBool32 False = new VkBool32(0);

        public static implicit operator bool(VkBool32 b) => b.Value != 0;
        public static implicit operator uint(VkBool32 b) => b.Value;
        public static implicit operator VkBool32(bool b) => b ? True : False;
        public static implicit operator VkBool32(uint value) => new VkBool32(value);

        public override string ToString()
        {
            return $"{(this ? "true" : "false")} ({Value})";
        }
    }
}
