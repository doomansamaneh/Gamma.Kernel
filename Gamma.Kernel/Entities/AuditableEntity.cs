
using Gamma.Kernel.Attributes;

namespace Gamma.Kernel.Entities;

public abstract class AuditableEntity : BaseEntity
{
    [InsertOnly]
    public string CreatedBy { get; set; } = default!;

    [InsertOnly]
    public DateTime DateCreated { get; set; }

    public string ModifiedBy { get; set; } = default!;

    public DateTime DateModified { get; set; }
}

