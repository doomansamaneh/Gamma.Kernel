using Gamma.Kernel.Models;

namespace Gamma.Kernel.Abstractions;

public interface ICommandHandler<in TCommand, TResultData>
{
    Task<Result<TResultData>> Handle(TCommand command, CancellationToken cancellationToken = default);
}

