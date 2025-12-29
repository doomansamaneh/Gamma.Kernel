using Gamma.Kernel.Commands;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface ICreateCommandHandler<TEntity> : ICommandHandler<GenericCreateCommand<TEntity>, Guid>
    where TEntity : BaseEntity;
