using System.Data;
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public class DapperUnitOfWork : IUnitOfWork
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

    private bool _completed;
    public Task CommitAsync()
    {
        if (_completed) throw new InvalidOperationException("UnitOfWork already completed.");
        _transaction?.Commit();
        _completed = true;
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        _transaction?.Rollback();
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _transaction?.Dispose();
        if (_connection.State == ConnectionState.Open)
            _connection.Close();
    }
}
