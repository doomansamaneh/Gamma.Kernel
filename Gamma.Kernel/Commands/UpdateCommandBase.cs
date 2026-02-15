using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract record UpdateCommandBase<TEntity>
    : ICommand<Result<int>>
    where TEntity : BaseEntity
{
    public Guid Id { get; set; }
}


