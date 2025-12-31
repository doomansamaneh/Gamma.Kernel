
using Gamma.Kernel.Attributes;

namespace Gamma.Kernel.Entities;

public abstract class ConcurrencyEntity : AuditableEntity
{
    [RowVersion]
    public long RowVersion { get; set; } = 1;
}

