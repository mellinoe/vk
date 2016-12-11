using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Vk.Generator
{
    public class VulkanSpecification
    {
        public CommandDefinition[] Commands { get; }
        public EnumDefinition[] Enums { get; }
        public StructureDefinition[] Structures { get; }
        public StructureDefinition[] Unions{ get; }
        public HandleDefinition[] Handles { get; }
        public string[] BitmaskTypes { get; }
        public Dictionary<string, string> BaseTypes { get; }

        public VulkanSpecification(
            CommandDefinition[] commands,
            EnumDefinition[] enums,
            StructureDefinition[] structures,
            StructureDefinition[] unions,
            HandleDefinition[] handles,
            string[] bitmaskTypes,
            Dictionary<string, string> baseTypes)
        {
            Commands = commands;
            Enums = enums;
            Structures = structures;
            Unions = unions;
            Handles = handles;
            BitmaskTypes = bitmaskTypes;
            BaseTypes = baseTypes;
        }

        public static VulkanSpecification LoadFromXmlStream(Stream specFileStream)
        {
            var spec = XDocument.Load(specFileStream);
            var registry = spec.Element("registry");
            var commands = registry.Element("commands");
            CommandDefinition[] commandDefinitions = commands.Elements("command")
                .Select(commandx => CommandDefinition.CreateFromXml(commandx)).ToArray();

            EnumDefinition[] enumDefinitions = registry.Elements("enums")
                .Where(enumx => enumx.Attribute("name").Value != "API Constants")
                .Select(enumx => EnumDefinition.CreateFromXml(enumx)).ToArray();

            var types = registry.Elements("types");
            StructureDefinition[] structures = types.Elements("type").Where(typex => typex.HasCategoryAttribute("struct"))
                .Select(typex => StructureDefinition.CreateFromXml(typex)).ToArray();

            StructureDefinition[] unions = types.Elements("type").Where(typex => typex.HasCategoryAttribute("union"))
                .Select(typex => StructureDefinition.CreateFromXml(typex)).ToArray();

            HandleDefinition[] handles = types.Elements("type").Where(typex => typex.HasCategoryAttribute("handle"))
                .Select(typex => HandleDefinition.CreateFromXml(typex)).ToArray();

            string[] bitmaskTypes = types.Elements("type").Where(typex => typex.HasCategoryAttribute("bitmask"))
                .Select(typex => typex.GetNameElement()).ToArray();

            Dictionary<string, string> baseTypes = types.Elements("type").Where(typex => typex.HasCategoryAttribute("basetype"))
                .ToDictionary(
                    typex => typex.GetNameElement(),
                    typex => typex.Element("type").Value);

            return new VulkanSpecification(commandDefinitions, enumDefinitions, structures, unions, handles, bitmaskTypes, baseTypes);
        }
    }
}
