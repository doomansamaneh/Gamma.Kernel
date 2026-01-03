using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Commands;

public sealed record GenericDeleteCommand<TEntity, TKey>(TKey Id) : ICommand<Result<int>>
    where TEntity : BaseEntity
{
    public TKey Id { get; } = Id ?? throw new ArgumentNullException(nameof(Id));
}

