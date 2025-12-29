using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Enums;

namespace Gamma.Next.Application.Commands.ProductGroup;

public class DeleteProductGroupCommand : IAuditableCommand
{
    public Guid Id { get; set; }
    public AuditAction Action => AuditAction.Delete;
    public string EntityName => "Ast.ProductGroup";
    public string EntityId => Id.ToString();
    public object? Before => null;
    public object? After => null;
}