using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;
using Gamma.Next.Application.Commands.Shared;

namespace Gamma.Next.Application.Commands.ProductGroup;

public class DeleteProductGroupCommand : DeleteCommand, IAuditableCommand
{
    public AuditAction Action => AuditAction.Delete;

    public string EntityName => "Ast.ProductGroup";

    public string EntityId => Id.ToString();

    public object? Before => null;
    public object? After => null;
}