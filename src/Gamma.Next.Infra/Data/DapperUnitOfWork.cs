using System.Data;
using Gamma.Kernel.Abstractions;

namespace Gamma.Next.Infra.Data;

internal class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public IDbConnection Connection => _connection;
    public IDbTransaction Transaction => _transaction!;

    public DapperUnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        if (_connection.State != ConnectionState.Open) _connection.Open();
        _transaction = _connection.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        _transaction?.Commit();
        await DisposeAsync();
    }

    public async Task RollbackAsync()
    {
        _transaction?.Rollback();
        await DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _transaction?.Dispose();
        if (_connection.State == ConnectionState.Open)
            _connection.Close();
    }
}
