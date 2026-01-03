using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public sealed record GenericUpdateCommand<TEntity>(TEntity Entity) : ICommand<Result<int>>
    where TEntity : BaseEntity
{
    public TEntity Entity { get; } = Entity ?? throw new ArgumentNullException(nameof(Entity));
}

