using Gamma.Kernel.Models;

namespace Gamma.Next.Application.Interfaces;

public interface ICommandService<TAddEntityCommand, TEditEntityCommand, TDeleteEntityCommand, TKey>
{
    Task<Result<TKey>> AddAsync(TAddEntityCommand command, CancellationToken cancellationToken = default);
    Task<Result<int>> EditAsync(TEditEntityCommand command, CancellationToken cancellationToken = default);
    Task<Result<int>> DeleteAsync(TDeleteEntityCommand command, CancellationToken cancellationToken = default);
}
