using System.Data;
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public IDbConnection Connection => _connection;
    public IDbTransaction Transaction => _transaction!;

    private readonly List<Func<CancellationToken, Task>> _onCommitted = [];

    public DapperUnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        if (_connection.State != ConnectionState.Open) _connection.Open();
        _transaction = _connection.BeginTransaction();
    }

    public void OnCommitted(Func<CancellationToken, Task> action)
    {
        if (_completed) throw new InvalidOperationException("Cannot register OnCommitted after completion.");

        _onCommitted.Add(action);
    }

    private bool _completed;
    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_completed) throw new InvalidOperationException("UnitOfWork already completed.");
        _transaction?.Commit();
        _completed = true;
        foreach (var action in _onCommitted)
            await action(ct);
    }

    public Task RollbackAsync(CancellationToken ct = default)
    {
        if (_completed) return Task.CompletedTask;

        _transaction?.Rollback();
        _completed = true;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _transaction?.Dispose();
        if (_connection.State == ConnectionState.Open)
            _connection.Close();
    }
}
