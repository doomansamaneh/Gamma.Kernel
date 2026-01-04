namespace Gamma.Kernel.Logging;

public sealed class AuditableScope : IDisposable
{
    private static readonly AsyncLocal<string?> _current = new();

    private readonly bool _isOwner;

    private AuditableScope(bool isOwner)
    {
        _isOwner = isOwner;
    }

    /// <summary>
    /// True if a correlation scope already exists
    /// </summary>
    public static bool HasCurrent => _current.Value != null;

    /// <summary>
    /// Current CorrelationId (throws if accessed outside scope)
    /// </summary>
    public static string Current =>
        _current.Value ?? throw new InvalidOperationException("No AuditableScope active.");

    /// <summary>
    /// Creates a new correlation scope if none exists.
    /// Root commands own the scope; nested commands reuse it.
    /// </summary>
    public static AuditableScope Create()
    {
        var isOwner = _current.Value == null;

        if (isOwner)
            _current.Value = Guid.NewGuid().ToString();

        return new AuditableScope(isOwner);
    }

    public void Dispose()
    {
        if (_isOwner)
            _current.Value = null;
    }
}
