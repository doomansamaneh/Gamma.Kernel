using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Services;

internal sealed class AuthorizationService(ICurrentUser currentUser) : IAuthorizationService
{
    public Task<bool> HasPermissionAsync(string permission, CancellationToken ct = default)
    {
        return Task.FromResult(currentUser.Permissions.Contains(permission));
    }
}

