using Gamma.Kernel.Commands;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Abstractions;

public interface IDeleteCommandHandler<TEntity, TKey>
    : ICommandHandler<GenericDeleteCommand<TEntity, TKey>, Result<int>>
    where TEntity : BaseEntity;

