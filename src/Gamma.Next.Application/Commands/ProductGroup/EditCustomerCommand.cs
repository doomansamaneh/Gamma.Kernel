namespace Gamma.Next.Application.Commands.ProductGroup;

public class EditProductGroupCommand
{
    public Guid Id { get; set; }
    public long RecordVersion { get; set; }
    public ProductGroupInput ProductGroup { get; set; } = new();
}