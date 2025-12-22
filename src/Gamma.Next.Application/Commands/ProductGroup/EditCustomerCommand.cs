using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Next.Application.Commands.ProductGroup;

public class EditProductGroupCommand : IAuditableCommand
{
    public Guid Id { get; set; }
    public long RecordVersion { get; set; }
    public ProductGroupInput ProductGroup { get; set; } = new();

    public AuditAction Action => AuditAction.Update;

    public string EntityName => "Ast.ProductGroup";

    public string EntityId => Id.ToString();

    public object? Before => null;
    public object? After => ProductGroup;
}
