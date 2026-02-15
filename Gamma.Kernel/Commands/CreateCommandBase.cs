using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public abstract record CreateCommandBase<TEntity>
    : ICommand<Result<Guid>>
    where TEntity : BaseEntity
{
    //public abstract TEntity GetEntity();
}