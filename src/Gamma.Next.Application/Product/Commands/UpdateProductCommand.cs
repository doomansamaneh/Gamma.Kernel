using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Next.Application.Product.Commands;

public class UpdateProductCommand : IAuditableCommand
{
    public Guid Id { get; set; }
    public long RecordVersion { get; set; }
    public ProductInput Product { get; set; } = new();
    public AuditAction Action => AuditAction.Update;

    public string EntityName => "Ast.Product";

    public string EntityId => Id.ToString();

    public object? Before => null;
    public object? After => Product;
}
