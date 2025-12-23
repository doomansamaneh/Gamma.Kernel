namespace Gamma.Kernel.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequiresPermissionAttribute(string permission, string? resource = null) : Attribute
{
    public string Permission { get; } = permission;
    public string? Resource { get; } = resource;
}
