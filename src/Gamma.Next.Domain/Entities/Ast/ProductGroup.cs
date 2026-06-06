using Gamma.Kernel.Attributes;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Exceptions;

namespace Gamma.Next.Domain.Entities.Ast;

[Schema("Ast")]
public partial class ProductGroup : ConcurrencyEntity
{
    [Identity]
    public long RowNo { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;
}
