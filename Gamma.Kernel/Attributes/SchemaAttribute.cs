namespace Gamma.Kernel.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SchemaAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
