namespace Vk.Generator
{
    public class TypeSpec
    {
        public string Name { get; }
        public int PointerIndirection { get; }

        public TypeSpec(string name) : this(name, 0) { }
        public TypeSpec(string name, int pointerIndirection)
        {
            Name = name;
            PointerIndirection = pointerIndirection;
        }

        public TypeSpec MapTypeSpec(TypeNameMappings tnm)
        {
            return new TypeSpec(tnm.GetMappedName(Name), PointerIndirection);
        }

        public override string ToString()
        {
            return GetFullTypeName();
        }

        private string GetFullTypeName()
        {
            return $"{Name}{new string('*', PointerIndirection)}";
        }
    }
}
