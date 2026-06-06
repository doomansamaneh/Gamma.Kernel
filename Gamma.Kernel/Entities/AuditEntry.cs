using Gamma.Kernel.Enums;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Entities;

public sealed class AuditEntry
{
    public string? CorrelationId { get; set; }
    public string EntityName { get; init; } = default!;
    public string EntityId { get; init; } = default!;
    public object? Payload { get; set; }
    public AuditAction Action { get; init; }
    public object? Before { get; init; }
    public object? After { get; init; }
    public LogActorModel Actor { get; set; } = default!;
    public DateTime LogTime { get; set; }
}
