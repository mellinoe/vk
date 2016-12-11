using System;

namespace Vk.Generator
{
    public static class HandleHelpers
    {
        public static void WriteHandle(CsCodeWriter cw, HandleDefinition handle)
        {
            if (handle.Parent != null)
            {
                cw.WriteIndentation();
                cw.Write($"///<summary>");
                cw.Write($"A {(handle.Dispatchable ? "dispatchable" : "non-dispatchable")} handle owned by a {handle.Parent}.");
                cw.Write("</summary>");
                cw.Write(Environment.NewLine);
            }
            using (cw.PushBlock("public partial struct " + handle.Name))
            {
                cw.WriteLine("public readonly IntPtr Handle;");
            }
        }
    }
}