using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Next.Application.Commands.ProductGroup;

internal class EditProductGroupHandler(IRepository<Domain.Entities.ProductGroup> repository)
    : ICommandHandler<EditProductGroupCommand, int>
{
    private readonly IRepository<Domain.Entities.ProductGroup> _repository = repository;
    public async Task<Result<int>> Handle(EditProductGroupCommand command,
         CancellationToken ct = default)
    {
        var entity = new Domain.Entities.ProductGroup
        {
            Id = command.Id,
            //RowVersion = command.RecordVersion,
            Code = command.ProductGroup.Code,
            Title = command.ProductGroup.Title,
            Comment = command.ProductGroup.Comment,
            IsActive = command.ProductGroup.IsActive
        };

        var affected = await _repository.UpdateAsync(entity, ct);

        return Result<int>.Ok(affected);
    }
}
