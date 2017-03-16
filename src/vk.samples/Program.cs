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
            //TriangleExample.RunSample();
            new Pipelines.PipelinesExample().ExampleMain();
        }
    }
}