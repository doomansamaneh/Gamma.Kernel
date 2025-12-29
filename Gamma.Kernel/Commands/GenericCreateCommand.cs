using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Commands;

public sealed record GenericCreateCommand<TEntity>(TEntity Entity)
    where TEntity : BaseEntity
{
    public TEntity Entity { get; } = Entity ?? throw new ArgumentNullException(nameof(Entity));
}

