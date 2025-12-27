using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Next.Application.Commands.Person;

internal class AddPersonHandler
    (IRepository<Domain.Entities.Person> repository,
    IUidGenerator uidGenerator)
    : ICommandHandler<AddPersonCommand, Guid>
{
    public async Task<Result<Guid>> Handle(IUnitOfWork uow,
        AddPersonCommand command,
         CancellationToken ct = default)
    {
        var entity = new Domain.Entities.Person
        {
            Id = uidGenerator.New(),
            NationalCode = command.Person.NationalCode,
            Name = command.Person.Name,
            LastName = command.Person.LastName,
            IsActive = command.Person.IsActive
        };

        await repository.InsertAsync(uow, entity, ct);

        return Result<Guid>.Ok(entity.Id);
    }
}