using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Next.Application.ProductGroup.Commands;

public sealed record UpdateProductGroupCommand(ProductGroupInput ProductGroup) : IAuditableCommand
{
    public Guid Id { get; set; }
    public long RowVersion { get; set; }
    public AuditAction Action => AuditAction.Update;
    public string EntityName => "Ast.ProductGroup";
    public string EntityId => Id.ToString();
    public object? Before => null;
    public object? After => ProductGroup;
}
