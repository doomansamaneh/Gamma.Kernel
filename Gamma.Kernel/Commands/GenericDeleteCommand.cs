using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Commands;

public sealed record GenericDeleteCommand<TEntity, TKey>(TKey Id)
    where TEntity : BaseEntity
{
    public TKey Id { get; } = Id ?? throw new ArgumentNullException(nameof(Id));
}

