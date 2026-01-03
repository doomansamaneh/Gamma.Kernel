using System.Data;

namespace Gamma.Kernel.Abstractions;

public interface IUnitOfWork //: IAsyncDisposable
{
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; }
}



