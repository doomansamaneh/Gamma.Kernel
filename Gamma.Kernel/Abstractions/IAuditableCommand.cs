using Gamma.Kernel.Enums;

namespace Gamma.Kernel.Abstractions;

public interface ICommand
{

}

public interface IAuditableCommand
{
    AuditAction Action { get; }
    string EntityName { get; }
    string EntityId { get; }
    object? Before { get; }
    object? After { get; }
}
