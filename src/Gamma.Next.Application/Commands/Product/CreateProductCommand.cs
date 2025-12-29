using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;

namespace Gamma.Next.Application.Commands.Product;

public sealed record CreateProductCommand(ProductInput Product) : IAuditableCommand, ICommand
{
    public AuditAction Action => AuditAction.Create;
    public string EntityName => "Ast.Product";
    public string EntityId => "";
    public object? Before => null;
    public object? After => Product;
}