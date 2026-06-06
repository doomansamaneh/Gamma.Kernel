namespace Gamma.Kernel.Abstractions;

public interface IAuthorizationService
{
    /// <summary>
    /// current user permission
    /// </summary>
    /// <param name="permission"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> HasPermissionAsync(string permission, CancellationToken ct = default);

    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken ct = default);

    /// <summary>
    /// currrent user peermission
    /// </summary>
    /// <param name="permissions"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> HasAnyPermissionAsync(string[] permissions, CancellationToken ct = default);
    /// <summary>
    /// current user permission
    /// </summary>
    /// <param name="permissions"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> HasAllPermissionsAsync(string[] permissions, CancellationToken ct = default);

    /// <summary>
    /// current user
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<HashSet<string>> GetUserPermissionsAsync(CancellationToken ct = default);
    Task<HashSet<string>> GetUserPermissionsAsync(Guid userId, CancellationToken ct = default);

    void RefreshPermissions(Guid userId);
    /// <summary>
    /// current user refresh
    /// </summary>
    void RefreshCurrentUserPermissions();
}

