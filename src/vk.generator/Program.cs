using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Vk.Generator
{
    public class Program
    {
        public static void Main()
        {
            using (var fs = File.OpenRead("vk.xml"))
            {
                VulkanSpecification vs = VulkanSpecification.LoadFromXmlStream(fs);
                TypeNameMappings tnm = new TypeNameMappings();
                foreach (var bitmask in vs.BitmaskTypes)
                {
                    tnm.AddMapping(bitmask, "uint");
                }
                foreach (var baseType in vs.BaseTypes)
                {
                    tnm.AddMapping(baseType.Key, baseType.Value);
                }
                CodeGenerator.GenerateCodeFiles(vs, tnm, AppContext.BaseDirectory);
            }
        }
    }
}