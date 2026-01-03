using Gamma.Kernel.Enums;
using Mediator;

namespace Gamma.Kernel.Abstractions;

public interface IAuditableCommand : IBaseCommand
{
    AuditAction Action { get; }
    string EntityName { get; }
    string EntityId { get; }
    object? Before { get; }
    object? After { get; }
}
