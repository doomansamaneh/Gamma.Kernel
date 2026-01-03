using Gamma.Kernel.Commands;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Abstractions;

public interface IUpdateCommandHandler<TEntity>
    : ICommandHandler<GenericUpdateCommand<TEntity>, Result<int>>
    where TEntity : BaseEntity;

