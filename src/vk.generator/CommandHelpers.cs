using System.Linq;
using System.Runtime.InteropServices;

namespace Vk.Generator
{
    public static class CommandHelpers
    {
        public static void WriteCommand(CsCodeWriter cw, TypeNameMappings tnm, CommandDefinition command)
        {
            if (Configuration.GenerateCalliStubs)
            {
                cw.WriteLine($"private static IntPtr {command.Name}_ptr;");
            }
            else
            { 
                cw.WriteLine($"private delegate {command.ReturnType.MapTypeSpec(tnm)} {command.Name}_delegate({command.GetParametersSignature(tnm)});");
                cw.WriteLine($"private static {command.Name}_delegate {command.Name}_ptr;");
            }

            if (command.SuccessCodes.Length != 0)
            {
                cw.WriteLine($"///<remarks>Success codes:{string.Join(", ", command.SuccessCodes)}. Error codes:{string.Join(", ", command.ErrorCodes)}</remarks>");
            }

            if (Configuration.GenerateCalliStubs)
            {
                cw.WriteLine("[Generator.CalliRewrite]");
            }
            using (cw.PushBlock($"public static unsafe {command.ReturnType.MapTypeSpec(tnm)} {command.Name}({command.GetParametersSignature(tnm)})"))
            {
                if (Configuration.GenerateCalliStubs)
                {
                    cw.WriteLine("throw new NotImplementedException();");
                }
                else
                {
                    string invocation = string.Join(", ", command.Parameters.Select(pd => Util.NormalizeName(pd.Name)));
                    string ret = (command.ReturnType.Name == "void") ? "" : "return ";

                    cw.WriteLine($"{ret}{command.Name}_ptr({invocation});");
                }
            }
        }
    }
}
