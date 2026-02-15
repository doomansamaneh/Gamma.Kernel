using Gamma.Kernel.Attributes;
using Gamma.Kernel.Entities;
using Gamma.Kernel.Exceptions;

namespace Gamma.Next.Domain.Entities;

[Schema("Ast")]
public class ProductGroup : ConcurrencyEntity
{
    [Identity]
    public long RowNo { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;

    public static ProductGroup Create(
        string code,
        string title,
        string? comment,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Code is required");

        return new ProductGroup
        {
            Code = code,
            Title = title,
            Comment = comment,
            IsActive = isActive
        };
    }
}
