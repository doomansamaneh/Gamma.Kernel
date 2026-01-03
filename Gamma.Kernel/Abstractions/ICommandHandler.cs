using Gamma.Kernel.Models;

namespace Gamma.Kernel.Abstractions;

public interface ICommandHandler_<in TCommand, TResultData>
{
    Task<Result<TResultData>> HandleAsync(IUnitOfWork uow, TCommand command, CancellationToken ct = default);
}
