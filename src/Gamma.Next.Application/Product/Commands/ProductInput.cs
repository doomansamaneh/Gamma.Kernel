namespace Gamma.Next.Application.Product.Commands;

public class ProductInput
{
    public Guid ProductGroupId { get; set; }
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;

    public Domain.Entities.Product ToEntity()
    {
        return new Domain.Entities.Product
        {
            Code = Code,
            ProductGroupId = ProductGroupId,
            Title = Title,
            Comment = Comment,
            IsActive = IsActive
        };
    }
}