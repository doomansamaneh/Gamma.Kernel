using System.Data;
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Persistence;

public sealed class UnitOfWork(
    IDbConnection connection,
    IDbTransaction? transaction) : IUnitOfWork
{
    public IDbConnection Connection { get; } = connection ?? throw new ArgumentNullException(nameof(connection));
    public IDbTransaction? Transaction { get; } = transaction;

    private readonly List<Func<CancellationToken, Task>> _onCommitted = [];
    private bool _committed;

    public void OnCommitted(Func<CancellationToken, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (_committed)
            throw new InvalidOperationException("Cannot register OnCommitted after commit.");

        _onCommitted.Add(action);
    }

    public async Task NotifyCommittedAsync(CancellationToken ct = default)
    {
        if (_committed) return;

        _committed = true;

        foreach (var action in _onCommitted)
            await action(ct);
    }
}

