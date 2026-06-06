namespace Gamma.Kernel.Security;

public sealed record PermissionModel(
    string Module,
    string Resource,
    string Action)
{
    public string Key => $"{Module}.{Resource}.{Action}";
}
