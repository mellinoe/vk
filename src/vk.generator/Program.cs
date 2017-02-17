using System;
using System.IO;
using System.CommandLine;

namespace Vk.Generator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            string outputPath = AppContext.BaseDirectory;

            ArgumentSyntax.Parse(args, s =>
            {
                s.DefineOption("o|out", ref outputPath, "The folder into which code is generated. Defaults to the application directory.");
            });

            Configuration.CodeOutputPath = outputPath;

            if (File.Exists(outputPath))
            {
                Console.Error.WriteLine("The given path is a file, not a folder.");
                return 1;
            }
            else if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

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

                CodeGenerator.GenerateCodeFiles(vs, tnm, Configuration.CodeOutputPath);

                Configuration.GenerateCalliStubs = true;
                Configuration.CodeOutputPath = Configuration.CodeOutputPath + ".calli";
                CodeGenerator.GenerateCodeFiles(vs, tnm, Configuration.CodeOutputPath);
            }

            return 0;
        }
    }
}