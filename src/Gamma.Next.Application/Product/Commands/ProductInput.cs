namespace Gamma.Next.Application.Product.Commands;

public class ProductInput
{
    public Guid ProductGroupId { get; set; }
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;
}