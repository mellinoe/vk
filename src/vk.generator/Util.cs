using System;
using System.Collections.Generic;

namespace Vk.Generator
{
    public static class Util
    {
        private static readonly Dictionary<string, int> s_typeSizes = new Dictionary<string, int>
        {
            { "byte", 1 },
            { "uint", 4 },
            { "ulong", 4 },
            { "int", 4 },
            { "float", 4 },
        };

        private static readonly HashSet<string> s_keywords = new HashSet<string>
        {
            "object",
            "event",
        };

        public static int GetTypeSize(TypeSpec type)
        {
            if (type.PointerIndirection != 0 || !s_typeSizes.TryGetValue(type.Name, out int size))
            {
                throw new InvalidOperationException();
            }

            return size;
        }

        public static string NormalizeName(string name)
        {
            if (s_keywords.Contains(name))
            {
                return "@" + name;
            }

            return name;
        }
    }
}