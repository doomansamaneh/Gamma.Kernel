using System.Data;

namespace Gamma.Kernel.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
    void OnCommitted(Func<CancellationToken, Task> action);
}



