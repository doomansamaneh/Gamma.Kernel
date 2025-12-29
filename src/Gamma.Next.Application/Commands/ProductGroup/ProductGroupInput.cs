using Gamma.Next.Application.Commands.Product;

namespace Gamma.Next.Application.Commands.ProductGroup;

public sealed record ProductGroupInput
{
    public string Code { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Comment { get; init; }
    public bool IsActive { get; init; } = true;

    public IList<ProductInput> Products { get; init; } = [];
}
