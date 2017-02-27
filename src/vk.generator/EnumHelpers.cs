using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vk.Generator
{
    public static class EnumHelpers
    {
        private static readonly char[] s_underscoreSeparator = { '_' };

        private static readonly Dictionary<string, string> s_knownEnumPrefixes = new Dictionary<string, string>
        {
            { "VkResult", "VK" },
        };

        private static readonly Dictionary<string, string> s_knownEnumValueNames = new Dictionary<string, string>
        {
            {  "VK_STENCIL_FRONT_AND_BACK", "FrontAndBack" }
        };

        private static readonly HashSet<string> s_ignoredParts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "flags",
            "nvx",
            "nv",
            "bit",
            "amd",
            "ext",
            "khr",
        };

        public static void WriteEnum(CsCodeWriter cw, EnumDefinition enumDef, TypeNameMappings tnm)
        {
            if (enumDef.Name.EndsWith("FlagBits"))
            {

            }
            if (enumDef.Type == EnumType.Bitmask)
            {
                cw.WriteLine("[Flags]");
            }
            string mappedName = tnm.GetMappedName(enumDef.Name);
            using (cw.PushBlock("public enum " + mappedName))
            {
                string enumNamePrefix = GetEnumNamePrefix(mappedName);
                if (enumDef.Type == EnumType.Bitmask && !enumDef.Values.Any(ev => GetPrettyEnumName(ev.Name, enumNamePrefix) == "None"))
                {
                    cw.WriteLine($"None = 0,");
                }
                foreach (var value in enumDef.Values)
                {
                    if (!string.IsNullOrEmpty(value.Comment))
                    {
                        cw.WriteLine($"///<summary>{value.Comment}</summary>");
                    }

                    string prettyName = GetPrettyEnumName(value.Name, enumNamePrefix);
                    cw.WriteLine($"{prettyName} = {value.ValueOrBitPosition},");
                }
            }
        }

        public static string GetEnumNamePrefix(string typeName)
        {
            if (s_knownEnumPrefixes.TryGetValue(typeName, out string knownValue))
            {
                return knownValue;
            }

            List<string> parts = new List<string>(4);
            int chunkStart = 0;
            for (int i = 0; i < typeName.Length; i++)
            {
                if (char.IsUpper(typeName[i]))
                {
                    if (chunkStart != i)
                    {
                        parts.Add(typeName.Substring(chunkStart, i - chunkStart));
                    }

                    chunkStart = i;
                    if (i == typeName.Length - 1)
                    {
                        parts.Add(typeName.Substring(i, 1));
                    }
                }
                else if (i == typeName.Length - 1)
                {
                    parts.Add(typeName.Substring(chunkStart, typeName.Length - chunkStart));
                }
            }

            for (int i = 0; i < parts.Count; i++)
            {
                if (
                        parts[i] == "Flag"
                    ||  parts[i] == "Flags"
                    || (parts[i] == "K" && (i + 2) < parts.Count && parts[i + 1] == "H" && parts[i + 2] == "R")
                    || (parts[i] == "A" && (i + 2) < parts.Count && parts[i + 1] == "M" && parts[i + 2] == "D")
                    || (parts[i] == "E" && (i + 2) < parts.Count && parts[i + 1] == "X" && parts[i + 2] == "T")
                    || (parts[i] == "Type" && (i + 3) < parts.Count && parts[i + 1] == "N" && parts[i + 2] == "V" && parts[i + 3] == "X")
                    )
                {
                    parts = new List<string>(parts.Take(i));
                    break;
                }
            }

            return string.Join("_", parts.Select(s => s.ToUpper()));
        }

        public static string TrimIgnoredParts(string ret)
        {
            foreach (string ignored in s_ignoredParts)
            {
                int index;
                if ((index = ret.IndexOf(ignored, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    ret = ret.Remove(index, ignored.Length);
                }
            }

            return ret;
        }

        public static string GetPrettyEnumName(string value, string enumPrefix)
        {
            if (s_knownEnumValueNames.TryGetValue(value, out string knownName))
            {
                return knownName;
            }

            if (value.IndexOf(enumPrefix) != 0)
            {
                return value;
            }

            string[] parts = value.Substring(enumPrefix.Length, value.Length - enumPrefix.Length).Split(s_underscoreSeparator, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            foreach (string part in parts)
            {
                if (s_ignoredParts.Contains(part))
                {
                    continue;
                }

                sb.Append(char.ToUpper(part[0]));
                for (int i = 1; i < part.Length; i++)
                {
                    sb.Append(char.ToLower(part[i]));
                }
            }

            string prettyName = sb.ToString();
            return (char.IsNumber(prettyName[0])) ? "_" + prettyName : prettyName;
        }
    }
}
