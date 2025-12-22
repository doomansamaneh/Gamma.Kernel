using Gamma.Kernel.Enums;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Entities;

public sealed class AuditEntry
{
    public string EntityName { get; init; } = default!;
    public string EntityId { get; init; } = default!;
    public AuditAction Action { get; init; }

    public LogActorModel Actor { get; init; } = default!;
    public DateTime LogTime { get; init; }

    public object? Before { get; init; }
    public object? After { get; init; }

    public string? CorrelationId { get; init; }
}
