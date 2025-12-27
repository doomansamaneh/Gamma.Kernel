using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;
using Gamma.Next.Application.Commands.Shared;

namespace Gamma.Next.Application.Commands.Person;

public class DeletePersonCommand : DeleteCommand, IAuditableCommand
{
    public AuditAction Action => AuditAction.Delete;

    public string EntityName => "Crm.Person";

    public string EntityId => Id.ToString();

    public object? Before => null;
    public object? After => null;
}