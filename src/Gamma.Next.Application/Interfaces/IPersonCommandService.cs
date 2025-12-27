using Gamma.Kernel.Abstractions;
using Gamma.Next.Application.Commands.Person;

namespace Gamma.Next.Application.Interfaces;

public interface IPersonCommandService : ICommandService<AddPersonCommand, EditPersonCommand, DeletePersonCommand, Guid>, IApplicationService
{

}
