namespace Gamma.Next.Application.Ast.Product.Dtos;

public sealed record ProductEditDto
{
    public Guid Id { get; init; }
    public Guid ProductGroupId { get; init; }
    public string Code { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Comment { get; init; }
    public bool IsActive { get; init; }

    public long RowVersion { get; init; }
    public string ProductGroupTitle { get; init; } = default!;
}
