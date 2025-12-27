using Gamma.Kernel.Abstractions;

namespace Gamma.Next.Application.Commands.Person;

public record AddPersonCommand(PersonInput Person) //: IAuditableCommand
{
}
