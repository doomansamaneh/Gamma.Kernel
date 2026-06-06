namespace Gamma.Next.Application.Ast.Product.Dtos;

public record ProductLookupDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string ProductGroupTitle { get; init; } = default!;
}
