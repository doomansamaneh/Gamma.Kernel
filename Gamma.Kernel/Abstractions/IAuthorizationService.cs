namespace Gamma.Kernel.Abstractions;

public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(
        string permission,
        string? resource = null,
        CancellationToken ct = default);
}
