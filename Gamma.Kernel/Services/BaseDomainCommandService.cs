
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Services;

public abstract class BaseDomainCommandService(IUnitOfWorkFactory unitOfWorkFactory)
{
    protected readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;

    protected async Task<Result<T>> ExecuteHandlerAsync<T>(Func<IUnitOfWork,
        Task<Result<T>>> action,
        CancellationToken ct = default)
    {
        await using var uow = _unitOfWorkFactory.Create();
        try
        {
            var result = await action(uow);

            if (result.Success) await uow.CommitAsync(ct);
            else await uow.RollbackAsync(ct);

            return result;
        }
        catch
        {
            await uow.RollbackAsync(ct);
            throw;
        }
    }
}
