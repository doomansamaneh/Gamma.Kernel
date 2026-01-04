using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Next.Application.Product.Commands;

public sealed record CreateProductCommand(ProductInput Product)
    : IAuditableCommand,
    ICommand<Result<Guid>>
{
    public AuditAction Action => AuditAction.Create;
    public string EntityName => "Ast.Product";
    public string EntityId => "";
    public object? Before => null;
    public object? After => Product;
}