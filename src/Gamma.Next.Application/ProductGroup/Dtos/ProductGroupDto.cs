namespace Gamma.Next.Application.ProductGroup.Dtos;

public sealed record ProductGroupDto
{
    public Guid Id { get; init; }
    public int RowNo { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Comment { get; init; }
    public bool IsActive { get; init; }
    public int RowVersion { get; init; }

    public ProductGroupDto() { }
}

