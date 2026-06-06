namespace Gamma.Kernel.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class AllowAnonymousAttribute() : Attribute
{
}
