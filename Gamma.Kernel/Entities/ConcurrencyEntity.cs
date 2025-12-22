
using Gamma.Kernel.Attributes;

namespace Gamma.Kernel.Entities;

public abstract class ConcurrencyEntity : AuditableEntity
{
    [RowVersion]
    [Computed]
    public int RowVersion { get; set; } = default!;
}

