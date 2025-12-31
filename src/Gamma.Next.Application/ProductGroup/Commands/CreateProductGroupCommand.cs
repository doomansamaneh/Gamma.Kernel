using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Next.Application.ProductGroup.Commands;

public sealed record CreateProductGroupCommand(ProductGroupInput ProductGroup) : IAuditableCommand
{
    public AuditAction Action => AuditAction.Create;
    public string EntityName => "Ast.ProductGroup";
    public string EntityId => "";
    public object? Before => null;
    public object? After => ProductGroup;
}