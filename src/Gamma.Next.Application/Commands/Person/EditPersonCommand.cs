using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Next.Application.Commands.Person;

public class EditPersonCommand : IAuditableCommand
{
    public Guid Id { get; set; }
    public long RecordVersion { get; set; }
    public PersonInput Person { get; set; } = new();

    public AuditAction Action => AuditAction.Update;

    public string EntityName => "Ast.Person";

    public string EntityId => Id.ToString();

    public object? Before => null;
    public object? After => Person;
}
