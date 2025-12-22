using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Next.Application.Commands.ProductGroup;

internal class DeleteProductGroupHandler(IRepository<Domain.Entities.ProductGroup> repository)
    : ICommandHandler<DeleteProductGroupCommand, int>
{
    private readonly IRepository<Domain.Entities.ProductGroup> _repository = repository;
    public async Task<Result<int>> Handle(IUnitOfWork uow,
        DeleteProductGroupCommand command,
        CancellationToken ct = default)
    {
        var affected = await _repository.DeleteByIdAsync(uow, command.Id, ct);
        return Result<int>.Ok(affected);
    }
}
