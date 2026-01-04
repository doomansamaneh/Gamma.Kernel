using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Security;

public sealed class AuthorizationContext : IAuthorizationContext
{
    public bool IsAuthorized { get; private set; }

    public void MarkAuthorized() => IsAuthorized = true;
}
