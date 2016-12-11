namespace Vk.Generator
{
    public static class StructureHelpers
    {
        public static void WriteStructure(CsCodeWriter cw, StructureDefinition structure, TypeNameMappings tnm)
        {
            using (cw.PushBlock("public unsafe partial struct " + structure.Name))
            {
                foreach (var member in structure.Members)
                {
                    if (member.ElementCount > 1)
                    {
                        for (int i = 0; i < member.ElementCount; i++)
                        {
                            WriteMember(cw, tnm, member, "_" + i);
                        }
                    }
                    else
                    {
                        WriteMember(cw, tnm, member, string.Empty);
                    }
                }
            }
        }

        private static void WriteMember(CsCodeWriter cw, TypeNameMappings tnm, MemberSpec member, string nameSuffix)
        {
            if (!string.IsNullOrEmpty(member.Comment))
            {
                cw.WriteLine($"///<summary>{member.Comment}</summary>");
            }

            cw.WriteLine($"public {member.Type.MapTypeSpec(tnm)} {Util.NormalizeName(member.Name)}{nameSuffix};");
        }
    }
}
