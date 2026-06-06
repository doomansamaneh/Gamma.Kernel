namespace Gamma.Next.Application.Ast.ProductGroup.Dtos;

public sealed record ProductGroupLookupDto
{
    public Guid Id {get; init;}
    public string Code { get; init; } = default!;
    public string Title { get; init; } = default!;

}
