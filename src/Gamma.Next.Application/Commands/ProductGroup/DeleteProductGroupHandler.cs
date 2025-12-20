using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;
using Gamma.Next.Application.Commands.Shared;

namespace Gamma.Next.Application.Commands.ProductGroup;

internal class DeleteProductGroupHandler(IRepository<Domain.Entities.ProductGroup> repository)
    : ICommandHandler<DeleteCommand, int>
{
    private readonly IRepository<Domain.Entities.ProductGroup> _repository = repository;
    public async Task<Result<int>> Handle(DeleteCommand command,
         CancellationToken ct = default)
    {
        var affected = await _repository.DeleteByIdAsync(command.Id, ct);
        return Result<int>.Ok(affected);
    }
}
