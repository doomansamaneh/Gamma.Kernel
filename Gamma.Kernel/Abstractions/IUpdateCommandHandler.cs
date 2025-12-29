using Gamma.Kernel.Commands;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IUpdateCommandHandler<TEntity> : ICommandHandler<GenericUpdateCommand<TEntity>, int>
    where TEntity : BaseEntity;

