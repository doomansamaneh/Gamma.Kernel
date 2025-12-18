namespace Gamma.Kernel.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class InsertOnlyAttribute : Attribute
{
}
