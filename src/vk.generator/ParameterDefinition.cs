using System;
using System.Xml.Linq;

namespace Vk.Generator
{
    public class ParameterDefinition
    {
        public string Name { get; }
        public TypeSpec Type { get;  }
        public bool IsOptional { get; }

        public ParameterDefinition(string name, TypeSpec type, bool isOptional)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
        }

        public static ParameterDefinition CreateFromXml(XElement xe)
        {
            string name = xe.Element("name").Value;
            var optionalAttr = xe.Attribute("optional");
            bool isOptional = optionalAttr != null && optionalAttr.Value == "true";
            string typeName = xe.Element("type").Value;
            int pointerLevel = 0;
            if (xe.Value.Contains($"{typeName}**") || xe.Value.Contains($"{typeName}* const*"))
            {
                pointerLevel = 2;
            }
            else if(xe.Value.Contains($"{typeName}*"))
            {
                pointerLevel = 1;
            }


            TypeSpec type = new TypeSpec(typeName, pointerLevel);

            return new ParameterDefinition(name, type, isOptional);
        }

        public string GetMappedAndNormalizedString(TypeNameMappings tnm)
        {
            return $"{Type.MapTypeSpec(tnm)} {Util.NormalizeName(Name)}";
        }

        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
