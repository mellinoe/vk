using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Vk.Generator
{
    public class VulkanSpecification
    {
        public CommandDefinition[] Commands { get; }
        public ConstantDefinition[] Constants { get; }
        public TypedefDefinition[] Typedefs { get; }
        public EnumDefinition[] Enums { get; }
        public StructureDefinition[] Structures { get; }
        public StructureDefinition[] Unions{ get; }
        public HandleDefinition[] Handles { get; }
        public string[] BitmaskTypes { get; }
        public Dictionary<string, string> BaseTypes { get; }
        public ExtensionDefinition[] Extensions { get; }

        public VulkanSpecification(
            CommandDefinition[] commands,
            ConstantDefinition[] constants,
            TypedefDefinition[] typedefs,
            EnumDefinition[] enums,
            StructureDefinition[] structures,
            StructureDefinition[] unions,
            HandleDefinition[] handles,
            string[] bitmaskTypes,
            Dictionary<string, string> baseTypes,
            ExtensionDefinition[] extensions)
        {
            Commands = commands;
            Constants = constants;
            Typedefs = typedefs;
            Enums = enums;
            Structures = structures;
            Unions = unions;
            Handles = handles;
            BitmaskTypes = bitmaskTypes;
            BaseTypes = baseTypes;
            Extensions = extensions;
            AddExtensionEnums(Enums, Extensions);
        }

        public static VulkanSpecification LoadFromXmlStream(Stream specFileStream)
        {
            var spec = XDocument.Load(specFileStream);
            var registry = spec.Element("registry");
            var commands = registry.Element("commands");
            CommandDefinition[] commandDefinitions = commands.Elements("command")
                .Where(xe => xe.Attribute("alias") == null)
                .Select(commandx => CommandDefinition.CreateFromXml(commandx)).ToArray();

            ConstantDefinition[] constantDefinitions = registry.Elements("enums")
                .Where(enumx => enumx.Attribute("name").Value == "API Constants")
                .SelectMany(enumx => enumx.Elements("enum"))
                .Where(enumx => enumx.Attribute("alias") == null)
                .Select(enumxx => ConstantDefinition.CreateFromXml(enumxx)).ToArray();

            var types = registry.Elements("types");
            TypedefDefinition[] typedefDefinitions = types.Elements("type").Where(xe => xe.Value.Contains("typedef") && xe.HasCategoryAttribute("bitmask"))
                .Select(xe2 => TypedefDefinition.CreateFromXml(xe2)).ToArray();

            EnumDefinition[] enumDefinitions = registry.Elements("enums")
                .Where(enumx => enumx.GetTypeAttributeOrNull() == "enum" || enumx.GetTypeAttributeOrNull() == "bitmask")
                .Where(enumx => enumx.Attribute("alias") == null)
                .Select(enumx => EnumDefinition.CreateFromXml(enumx)).ToArray();

            StructureDefinition[] structures = types.Elements("type").Where(typex => typex.HasCategoryAttribute("struct"))
                .Select(typex => StructureDefinition.CreateFromXml(typex)).ToArray();

            StructureDefinition[] unions =
                types.Elements("type")
                .Where(typex => typex.HasCategoryAttribute("union"))
                .Select(typex => StructureDefinition.CreateFromXml(typex)).ToArray();

            HandleDefinition[] handles = types.Elements("type").Where(typex => typex.HasCategoryAttribute("handle"))
                .Where(typex => typex.Attribute("alias") == null)
                .Select(typex => HandleDefinition.CreateFromXml(typex)).ToArray();

            string[] bitmaskTypes = types.Elements("type").Where(typex => typex.HasCategoryAttribute("bitmask"))
                .Where(typex => typex.Attribute("alias") == null)
                .Select(typex => typex.GetNameElement()).ToArray();

            Dictionary<string, string> baseTypes = types.Elements("type").Where(typex => typex.HasCategoryAttribute("basetype"))
                .Where(typex => typex.Attribute("alias") == null)
                .ToDictionary(
                    typex => typex.GetNameElement(),
                    typex => typex.Element("type").Value);

            ExtensionDefinition[] featureExtensions = registry.Elements("feature")
                .Select(xe => ExtensionDefinition.CreateFromXml(xe, 0)).ToArray();

            ExtensionDefinition[] extensionExtensions = registry.Element("extensions").Elements("extension")
                .Select(xe =>
                {
                    string numberString = xe.Attribute("number").Value;
                    int number = int.Parse(numberString);
                    return ExtensionDefinition.CreateFromXml(xe, number);
                }).ToArray();

            ExtensionDefinition[] extensions = featureExtensions.Concat(extensionExtensions).ToArray();

            return new VulkanSpecification(
                commandDefinitions,
                constantDefinitions,
                typedefDefinitions,
                enumDefinitions,
                structures,
                unions,
                handles,
                bitmaskTypes,
                baseTypes,
                extensions);
        }

        private void AddExtensionEnums(EnumDefinition[] enums, ExtensionDefinition[] extensions)
        {
            foreach (ExtensionDefinition exDef in extensions)
            {
                foreach (var enumEx in exDef.EnumExtensions)
                {
                    EnumDefinition enumDef = GetEnumDef(enums, enumEx.ExtendedType);
                    var prev = enumDef.Values.FirstOrDefault(val => val.Name == enumEx.Name);
                    int value = int.Parse(enumEx.Value);

                    if (prev != null)
                    {
                        if (prev.ValueOrBitPosition != value)
                        {
                            throw new InvalidOperationException("The same enum cannot have different values");
                        }
                        // Skip duplicates
                        continue;
                    }

                    enumDef.Values = enumDef.Values.Append(new EnumValue(enumEx.Name, value, null)).ToArray();
                }
            }
        }

        private EnumDefinition GetEnumDef(EnumDefinition[] enums, string name)
        {
            return enums.Single(ed => ed.Name == name);
        }
    }
}
