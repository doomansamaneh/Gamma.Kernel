using System.Data;

namespace Gamma.Kernel.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    Task CommitAsync();
    Task RollbackAsync();
}



