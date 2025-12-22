using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Entities;
using Serilog;

namespace Gamma.Next.Infra.Logging;

public sealed class SerilogAuditLogger(ILogger logger) : IAuditLogger
{
    public Task LogAsync(AuditEntry entry, CancellationToken ct = default)
    {
        logger.Information("Audit {@AuditEntry}", entry);

        return Task.CompletedTask;
    }
}
