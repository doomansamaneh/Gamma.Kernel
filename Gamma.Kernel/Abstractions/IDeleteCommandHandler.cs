using Gamma.Kernel.Commands;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IDeleteCommandHandler<TEntity, TKey>
    : ICommandHandler<GenericDeleteCommand<TEntity, TKey>, int>
    where TEntity : BaseEntity;
