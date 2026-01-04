using System.Data;
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Persistence;

public sealed class UnitOfWorkScope : IDisposable
{
    private static readonly AsyncLocal<IUnitOfWork?> _current = new();

    private readonly bool _isOwner;

    private UnitOfWorkScope(bool isOwner) => _isOwner = isOwner;

    public static bool HasCurrent => _current.Value != null;

    public static IUnitOfWork Current
        => _current.Value ?? throw new InvalidOperationException("No current UnitOfWork scope.");

    public static IUnitOfWork Begin(IDbConnection connection, IDbTransaction? transaction)
    {
        if (_current.Value != null)
            return _current.Value;

        var uow = new UnitOfWork(connection, transaction);
        _current.Value = uow;
        return uow;
    }

    public static UnitOfWorkScope CreateScope(IDbConnection? connection, IDbTransaction? transaction)
    {
        bool isOwner = _current.Value == null;
        if (isOwner)
        {
            ArgumentNullException.ThrowIfNull(connection);

            _current.Value = new UnitOfWork(connection, transaction);
        }

        return new UnitOfWorkScope(isOwner);
    }

    public void Dispose()
    {
        if (_isOwner)
        {
            _current.Value = null;
        }
    }
}


