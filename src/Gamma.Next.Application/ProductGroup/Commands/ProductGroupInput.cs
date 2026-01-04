using Gamma.Next.Application.Product.Commands;

namespace Gamma.Next.Application.ProductGroup.Commands;

public sealed record ProductGroupInput
{
    public string Code { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Comment { get; init; }
    public bool IsActive { get; init; } = true;

    public IList<ProductInput> Products { get; init; } = [];

    public Domain.Entities.ProductGroup ToEntity()
    {
        return new Domain.Entities.ProductGroup
        {
            Code = this.Code,
            Title = this.Title,
            Comment = this.Comment,
            IsActive = this.IsActive
        };
    }
}
