using Gamma.Kernel.Models;

namespace Gamma.Kernel.Abstractions;

public interface ICommandHandler<in TCommand, TResultData>
{
    Task<Result<TResultData>> Handle(IUnitOfWork uow, TCommand command, CancellationToken ct = default);
}
