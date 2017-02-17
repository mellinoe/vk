using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using Mono.Collections.Generic;

namespace Vk.Generator
{
    public class Program
    {
        private static TypeReference s_calliRewriteRef;

        public static void Main(string[] args)
        {
            string vkDllPath = null;
            string outputPath = null;

            var s = System.CommandLine.ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("vkdll", ref vkDllPath, "The location of vk.dll to rewrite.");
                syntax.DefineOption("out", ref outputPath, "The output location of the rewritten DLL. If not specified, the DLL is rewritten in-place.");
            });

            if (vkDllPath == null)
            {
                Console.WriteLine("Error: a path for --vkdll is required.");
                Console.WriteLine(s.GetHelpText());
            }
            if (outputPath == null)
            {
                outputPath = vkDllPath;
            }

            Rewrite(vkDllPath, outputPath);
        }

        private static void Rewrite(string vkDllPath, string outputPath)
        {
            AssemblyDefinition vkDll = AssemblyDefinition.ReadAssembly(vkDllPath);
            LoadRefs(vkDll);
            foreach (var type in vkDll.Modules[0].Types)
            {
                ProcessType(type);
            }
            vkDll.Write(outputPath);
        }

        private static void LoadRefs(AssemblyDefinition vkDll)
        {
            s_calliRewriteRef = vkDll.MainModule.GetType("Vulkan.Generator.CalliRewriteAttribute");
        }

        private static void ProcessType(TypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                ProcessMethod(method);
            }
        }

        private static void ProcessMethod(MethodDefinition method)
        {
            if (method.CustomAttributes.Any(ca => ca.AttributeType == s_calliRewriteRef))
            {
                var processor = method.Body.GetILProcessor();
                RewriteMethod(method);
                method.CustomAttributes.Remove(method.CustomAttributes.Single(ca => ca.AttributeType == s_calliRewriteRef));
            }
        }

        private static void RewriteMethod(MethodDefinition method)
        {
            var il = method.Body.GetILProcessor();
            il.Body.Instructions.Clear();

            for (int i = 0; i < method.Parameters.Count; i++)
            {
                EmitLoadArgument(il, i, method.Parameters);
            }

            string functionPtrName = method.Name + "_ptr";
            var field = method.DeclaringType.Fields.SingleOrDefault(fd => fd.Name == functionPtrName);
            if (field == null)
            {
                throw new InvalidOperationException("Can't find function pointer field for " + method.Name);
            }
            il.Emit(OpCodes.Ldsfld, field);

            CallSite callSite = new CallSite(method.ReturnType)
            {
                CallingConvention = MethodCallingConvention.StdCall
            };
            foreach (ParameterDefinition pd in method.Parameters)
            {
                callSite.Parameters.Add(pd);
            }
            il.Emit(OpCodes.Calli, callSite);
            il.Emit(OpCodes.Ret);

            if (method.Body.Variables.Count > 0)
            {
                method.Body.InitLocals = true;
            }
            
        }

        private static void EmitLoadArgument(ILProcessor il, int i, Collection<ParameterDefinition> parameters)
        {
            if (i == 0)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            else if (i == 1)
            {
                il.Emit(OpCodes.Ldarg_1);
            }
            else if (i == 2)
            {
                il.Emit(OpCodes.Ldarg_2);
            }
            else if (i == 3)
            {
                il.Emit(OpCodes.Ldarg_3);
            }
            else
            {
                il.Emit(OpCodes.Ldarg, i);
            }
        }
    }
}