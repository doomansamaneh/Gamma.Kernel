using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IAuditLogger
{
    Task LogAsync(AuditEntry entry, CancellationToken ct = default);
}