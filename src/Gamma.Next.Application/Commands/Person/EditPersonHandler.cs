using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Next.Application.Commands.Person;

internal class EditPersonHandler(IRepository<Domain.Entities.Person> repository)
    : ICommandHandler<EditPersonCommand, int>
{
    public async Task<Result<int>> Handle(IUnitOfWork uow,
     EditPersonCommand command,
         CancellationToken ct = default)
    {
        var entity = new Domain.Entities.Person
        {
            Id = command.Id,
            //RowVersion = command.RecordVersion,
            NationalCode = command.Person.NationalCode,
            Name = command.Person.Name,
            LastName = command.Person.LastName,
            IsActive = command.Person.IsActive
        };

        var affected = await repository.UpdateAsync(uow, entity, ct);

        return Result<int>.Ok(affected);
    }
}
