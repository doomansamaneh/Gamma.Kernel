using System.Reflection;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Exceptions;
using Gamma.Kernel.Models;
using Gamma.Kernel.Security;

namespace Gamma.Kernel.Behaviors;

public class CommandServiceAuthorizationDecorator<TAdd, TEdit, TDelete, TKey>(
    ICommandService<TAdd, TEdit, TDelete, TKey> inner,
    IAuthorizationService authorizationService) : ICommandService<TAdd, TEdit, TDelete, TKey>
{
    public async Task<Result<TKey>> AddAsync(TAdd command, CancellationToken ct = default)
    {
        await EnsurePermissionAsync(GetPermission(nameof(AddAsync)) ?? InferDefaultPermission("add"), ct);
        return await inner.AddAsync(command, ct);
    }

    public async Task<Result<int>> EditAsync(TEdit command, CancellationToken ct = default)
    {
        await EnsurePermissionAsync(GetPermission(nameof(EditAsync)) ?? InferDefaultPermission("edit"), ct);
        return await inner.EditAsync(command, ct);
    }

    public async Task<Result<int>> DeleteAsync(TDelete command, CancellationToken ct = default)
    {
        await EnsurePermissionAsync(GetPermission(nameof(DeleteAsync)) ?? InferDefaultPermission("delete"), ct);
        return await inner.DeleteAsync(command, ct);
    }

    private async Task EnsurePermissionAsync(string permission, CancellationToken ct)
    {
        var allowed = await authorizationService.HasPermissionAsync(permission, ct);
        if (!allowed) throw new ForbiddenException(permission);
    }

    private string? GetPermission(string methodName)
    {
        // 1. Look for attribute on concrete type
        var method = inner.GetType().GetMethod(methodName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null) return null;

        var attr = method.GetCustomAttribute<RequiresPermissionAttribute>(true);
        if (attr != null) return attr.Permission;

        // 2. Optionally look at class-level attributes
        attr = method.DeclaringType?.GetCustomAttribute<RequiresPermissionAttribute>(true);
        return attr?.Permission;
    }

    private static string InferDefaultPermission(string action)
    {
        // Example: extract entity name from generic TAdd type
        var entityName = typeof(TAdd).Name
            .Replace("Add", "")
            .Replace("Command", "")
            .ToLowerInvariant();

        return $"{entityName}.{action}";
    }
}

