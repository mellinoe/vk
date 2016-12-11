namespace Vk.Generator
{
    public static class CommandHelpers
    {
        public static void WriteCommand(CsCodeWriter cw, TypeNameMappings tnm, CommandDefinition command)
        {
            if (command.SuccessCodes.Length != 0)
            {
                cw.WriteLine($"///<remarks>Success codes:{string.Join(", ", command.SuccessCodes)}. Error codes:{string.Join(", ", command.ErrorCodes)}</remarks>");
            }

            cw.WriteLine($"[DllImport(VulkanLib)]");
            cw.WriteLine($"public static unsafe extern {command.ReturnType.MapTypeSpec(tnm)} {command.Name}({command.GetParametersSignature(tnm)});");
        }
    }
}
