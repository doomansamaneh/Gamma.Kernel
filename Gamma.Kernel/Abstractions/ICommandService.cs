using Gamma.Kernel.Models;

namespace Gamma.Kernel.Abstractions;

public interface ICommandService<TCreateCommand, TUpdateCommand, TDeleteCommand, TKey>
{
    Task<Result<TKey>> CreateAsync(TCreateCommand command, CancellationToken cancellationToken = default);
    Task<Result<int>> UpdateAsync(TUpdateCommand command, CancellationToken cancellationToken = default);
    Task<Result<int>> DeleteAsync(TDeleteCommand command, CancellationToken cancellationToken = default);
}
