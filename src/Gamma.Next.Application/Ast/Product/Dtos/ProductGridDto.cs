namespace Gamma.Next.Application.Ast.Product.Dtos;

public sealed record ProductGridDto : ProductLookupDto
{
    public string? Comment { get; init; }
    public bool IsActive { get; init; }
}
