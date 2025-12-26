namespace Gamma.Kernel.Abstractions;

public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(string permission, CancellationToken ct = default);
}
