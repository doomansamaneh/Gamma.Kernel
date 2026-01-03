using Gamma.Kernel.Commands;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Kernel.Abstractions;

public interface ICreateCommandHandler<TEntity>
    : ICommandHandler<GenericCreateCommand<TEntity>, Result<Guid>>
    where TEntity : BaseEntity;

