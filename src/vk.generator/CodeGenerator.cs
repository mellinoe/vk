using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vk.Generator
{
    public class CodeGenerator
    {
        public static void GenerateCodeFiles(VulkanSpecification spec, TypeNameMappings tnm, string path)
        {
            GenerateStructures(spec, tnm, path);
        }

        private static void GenerateStructures(VulkanSpecification spec, TypeNameMappings tnm, string path)
        {
            using (StreamWriter sw = File.CreateText(Path.Combine(path, "Structures.gen.cs")))
            {
                CsCodeWriter cw = new CsCodeWriter(sw);
                cw.WriteHeader();
                cw.WriteLine();

                cw.Using("System");
                cw.WriteLine();

                using (cw.PushBlock("namespace Vulkan"))
                {
                    SpaceSeparatedList(cw, spec.Structures, structure =>
                    {
                        StructureHelpers.WriteStructure(cw, structure, tnm);
                    });
                }
            }

            using (StreamWriter enumWriter = File.CreateText(Path.Combine(path, "Enums.gen.cs")))
            {
                CsCodeWriter cw = new CsCodeWriter(enumWriter);
                cw.WriteHeader();
                cw.WriteLine();

                cw.Using("System");
                cw.WriteLine();

                using (cw.PushBlock("namespace Vulkan"))
                {
                    SpaceSeparatedList(cw, spec.Enums, enumDef =>
                    {
                        EnumHelpers.WriteEnum(cw, enumDef);
                    });
                }
            }

            using (StreamWriter commandWriter = File.CreateText(Path.Combine(path, "Commands.gen.cs")))
            {
                CsCodeWriter cw = new CsCodeWriter(commandWriter);
                cw.WriteHeader();
                cw.WriteLine();

                cw.Using("System");
                cw.Using("System.Runtime.InteropServices");
                cw.WriteLine();

                using (cw.PushBlock("namespace Vulkan"))
                using (cw.PushBlock("public static unsafe partial class VulkanNative"))
                {
                    cw.WriteLine($"private const string VulkanLib = \"vulkan-1.dll\";");
                    cw.WriteLine();

                    SpaceSeparatedList(cw, spec.Commands, command =>
                    {
                        CommandHelpers.WriteCommand(cw, tnm, command);
                    });
                }
            }

            using (StreamWriter handleWriter = File.CreateText(Path.Combine(path, "Handles.gen.cs")))
            {
                CsCodeWriter cw = new CsCodeWriter(handleWriter);
                cw.WriteHeader();
                cw.WriteLine();

                cw.Using("System");
                cw.WriteLine();

                using (cw.PushBlock("namespace Vulkan"))
                {
                    SpaceSeparatedList(cw, spec.Handles, handle =>
                    {
                        HandleHelpers.WriteHandle(cw, handle);
                    });
                }
            }

            using (StreamWriter unionWriter = File.CreateText(Path.Combine(path, "Unions.gen.cs")))
            {
                CsCodeWriter cw = new CsCodeWriter(unionWriter);
                cw.WriteHeader();
                cw.WriteLine();

                cw.Using("System.Runtime.InteropServices");
                cw.WriteLine();

                using (cw.PushBlock("namespace Vulkan"))
                {
                    SpaceSeparatedList(cw, spec.Unions, union =>
                    {
                        UnionHelpers.WriteUnion(cw, tnm, union);
                    });
                }
            }
        }

        private static void SpaceSeparatedList<T>(CsCodeWriter cw, IList<T> list, Action<T> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                action(item);
                if (i != list.Count - 1)
                {
                    cw.WriteLine();
                }
            }
        }
    }

    public class CsCodeWriter
    {
        private readonly StreamWriter _sw;

        private int _indentLevel = 0;

        public CsCodeWriter(StreamWriter sw)
        {
            _sw = sw;
        }

        public void Using(string ns)
        {
            _sw.WriteLine($"using {ns};");
        }

        public void WriteHeader()
        {
            _sw.WriteLine("// This file is generated.");
        }

        public void PushBlock()
        {
            WriteLine('{');
            _indentLevel += 4;
        }

        public CodeBlock PushBlock(string ns)
        {
            return new CodeBlock(this, ns);
        }

        public void PopBlock()
        {
            _indentLevel -= 4;
            WriteLine('}');
        }

        public void WriteLine()
        {
            _sw.WriteLine();
        }

        public void WriteLine(string text)
        {
            WriteIndentation();
            _sw.WriteLine(text);
        }

        public void WriteLine(char c)
        {
            WriteIndentation();
            _sw.WriteLine(c);
        }

        public void Write(string text)
        {
            _sw.Write(text);
        }

        public void WriteIndentation()
        {
            for (int i = 0; i < _indentLevel; i++)
            {
                _sw.Write(' ');
            }
        }

        public class CodeBlock : IDisposable
        {
            private readonly CsCodeWriter _cw;

            public CodeBlock(CsCodeWriter cw, string header)
            {
                _cw = cw;
                _cw.WriteLine(header);
                _cw.PushBlock();
            }

            public void Dispose()
            {
                _cw.PopBlock();
            }
        }
    }
}
