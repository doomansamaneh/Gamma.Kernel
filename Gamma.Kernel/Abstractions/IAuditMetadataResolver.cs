using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Abstractions;

public interface IAuditMetadataResolver
{
    AuditEntry Resolve(object command);
}
