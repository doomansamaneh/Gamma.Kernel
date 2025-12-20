using Gamma.Kernel.Attributes;
using Gamma.Kernel.Entities;

namespace Gamma.Next.Domain.Entities;

[Schema("Ast")]
public class ProductGroup : ConcurrencyEntity
{
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;
}
