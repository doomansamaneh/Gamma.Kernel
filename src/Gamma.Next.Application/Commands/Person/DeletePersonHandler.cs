using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Next.Application.Commands.Person;

internal class DeletePersonHandler(IRepository<Domain.Entities.Person> repository)
    : ICommandHandler<DeletePersonCommand, int>
{
    public async Task<Result<int>> Handle(IUnitOfWork uow,
        DeletePersonCommand command,
        CancellationToken ct = default)
    {
        var affected = await repository.DeleteByIdAsync(uow, command.Id, ct);
        return Result<int>.Ok(affected);
    }
}
