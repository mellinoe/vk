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

                cw.WriteLine($"public {handle.Name}(IntPtr existingHandle) {{ Handle = existingHandle; }}");
                cw.WriteLine($"public static {handle.Name} Null => new {handle.Name}(IntPtr.Zero);");
                cw.WriteLine($"public static implicit operator {handle.Name}(IntPtr handle) => new {handle.Name}(handle);");
                cw.WriteLine($"public static bool operator ==({handle.Name} left, IntPtr right) => left.Handle == right;");
                cw.WriteLine($"public static bool operator !=({handle.Name} left, IntPtr right) => left.Handle != right;");
                cw.WriteLine($"public override bool Equals(object o) => o is IntPtr p && Handle.Equals(p);");
                cw.WriteLine($"public override int GetHashCode() => Handle.GetHashCode();");
            }
        }
    }
}