using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Enums;

namespace Gamma.Kernel.Logging;

public sealed class DefaultAuditMetadataResolver : IAuditMetadataResolver
{
    public AuditEntry Resolve(object command)
    {
        var type = command.GetType();

        return new AuditEntry
        {
            EntityName = type.Name.Replace("Command", ""),
            Action = ResolveAction(type.Name),
            EntityId = ResolveEntityId(command),
            Before = ResolveSnapshot(command, "Before"),
            After = ResolveSnapshot(command, "After"),
        };
    }

    private static AuditAction ResolveAction(string name)
        => name.StartsWith("Create") ? AuditAction.Create
         : name.StartsWith("Update") ? AuditAction.Update
         : name.StartsWith("Delete") ? AuditAction.Delete
         : AuditAction.Custom;

    private static string ResolveEntityId(object command)
        => command
            .GetType()
            .GetProperties()
            .FirstOrDefault(p => p.Name is "Id")
            ?.GetValue(command)
            ?.ToString()
            ?? string.Empty;

    private static object? ResolveSnapshot(object command, string propName)
        => command
            .GetType()
            .GetProperty(propName)
            ?.GetValue(command);
}
