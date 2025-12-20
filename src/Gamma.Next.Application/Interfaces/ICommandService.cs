using Gamma.Kernel.Models;
using Gamma.Next.Application.Commands.Shared;

namespace Gamma.Next.Application.Interfaces;

public interface ICommandService<TAddEntityCommand, TEditEntityCommand, TKey>
{
    Task<Result<TKey>> AddAsync(TAddEntityCommand command, CancellationToken cancellationToken = default);
    Task<Result<int>> EditAsync(TEditEntityCommand command, CancellationToken cancellationToken = default);
    Task<Result<int>> DeleteAsync(DeleteCommand command, CancellationToken cancellationToken = default);
}
