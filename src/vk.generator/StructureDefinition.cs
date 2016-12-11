using System.Linq;
using System.Xml.Linq;

namespace Vk.Generator
{
    public class StructureDefinition
    {
        public string Name { get; }
        public MemberSpec[] Members { get; }

        public StructureDefinition(string name, MemberSpec[] members)
        {
            Name = name;
            Members = members;
        }

        public static StructureDefinition CreateFromXml(XElement xe)
        {
            Require.NotNull(xe);

            string name = xe.GetNameAttribute();
            MemberSpec[] members = xe.Elements("member").Select(memberx => MemberSpec.CreateFromXml(memberx)).ToArray();
            return new StructureDefinition(name, members);
        }

        public override string ToString()
        {
            return $"struct {Name}[{Members.Length}]";
        }
    }

    public class MemberSpec
    {
        public string Name { get; }
        public TypeSpec Type { get; }
        public bool IsOptional { get; }
        public int ElementCount { get; }
        public string Comment { get; }

        public MemberSpec(string name, TypeSpec type, bool isOptional, int elementCount, string comment)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
            ElementCount = elementCount;
            Comment = comment;
        }

        public static MemberSpec CreateFromXml(XElement xe)
        {
            Require.NotNull(xe);

            string name = xe.GetNameElement();
            bool isOptional = xe.GetOptionalAttributeOrFalse();
            string typeName = xe.Element("type").Value;
            int pointerLevel = xe.Value.Contains($"{typeName}*") ? 1 : 0; // TODO: Make this better.
            if (xe.Value.Contains($"{typeName}* const*"))
            {
                pointerLevel += 1;
            }

            TypeSpec type = new TypeSpec(typeName, pointerLevel);

            int elementCount = 1;
            for (int i = 2; i < 10; i++)
            {
                if (xe.Value.Contains($"{name}[{i}]"))
                {
                    elementCount = i;
                    break;
                }
            }

            return new MemberSpec(name, type, isOptional, elementCount, string.Empty);
        }

        public override string ToString()
        {
            string optionalPart = IsOptional ? "[opt] " : "";
            return $"{optionalPart}{Type} {Name}";
        }
    }
}
