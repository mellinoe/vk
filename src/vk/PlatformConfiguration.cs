using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace Vulkan
{
	internal static class PlatformConfiguration
	{
		public static bool IsUnix { get; }
		public static bool IsWindows { get; }

		public static string DefaultAppDirectory { get; }

		static PlatformConfiguration()
		{
#if NET40 || NET45
			IsUnix = Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix;
			IsWindows = !IsUnix;

			var assemblyUri = new Uri(Assembly.GetEntryAssembly().CodeBase);
			DefaultAppDirectory = Path.GetDirectoryName(assemblyUri.LocalPath);
#else
			IsUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
			IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			DefaultAppDirectory = AppContext.BaseDirectory;
#endif
		}
	}
}
