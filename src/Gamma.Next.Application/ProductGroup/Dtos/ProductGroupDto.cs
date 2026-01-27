namespace Gamma.Next.Application.ProductGroup.Dtos;

public sealed record ProductGroupDto
{
    public Guid Id { get; init; } = default;
    public int RowNo { get; init; } = default;
    //public Guid? ParentId { get; init; } = null;
    public string Code { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Comment { get; init; } = null!;
    public bool IsActive { get; init; } = false;
    //public string CreatedBy { get; init; } = null!;
    //public DateTime DateCreated { get; init; } = DateTime.MinValue;
    // public string ModifiedBy { get; init; } = null!;
    // public DateTime DateModified { get; init; } = DateTime.MinValue;
    public int RowVersion { get; init; } = default;


    public ProductGroupDto() { }
}

