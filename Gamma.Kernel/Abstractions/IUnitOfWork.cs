using System.Data;

namespace Gamma.Kernel.Abstractions;

public interface IUnitOfWork //: IAsyncDisposable
{
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; }
    void OnCommitted(Func<CancellationToken, Task> action);
    Task NotifyCommittedAsync(CancellationToken ct = default);
}



