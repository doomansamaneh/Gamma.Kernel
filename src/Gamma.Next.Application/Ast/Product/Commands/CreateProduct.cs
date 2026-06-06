using FluentValidation;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.Product.Commands;

[RequiresPermission("ast.product.create")]
public sealed record CreateProductCommand(
    Guid ProductGroupId,
    string Code,
    string Title,
    string? Comment,
    bool IsActive
) : CreateCommandBase<Domain.Entities.Ast.Product>,
    IAuditableMessage,
    IProductCommand;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        Include(new ProductSharedValidator<CreateProductCommand>());
    }
}

public sealed class CreateProductCommandHandler(IProductRepository repository)
    : CreateCommandHandlerBase<CreateProductCommand, Domain.Entities.Ast.Product>(repository)
{
    protected override ValueTask<Domain.Entities.Ast.Product> CreateEntity(
        CreateProductCommand command, 
        CancellationToken ct)
    {
        var entity = Domain.Entities.Ast.Product.Create(
            productGroupId: command.ProductGroupId,
            code: command.Code,
            title: command.Title,
            comment: command.Comment,
            isActive: command.IsActive
        );

        return ValueTask.FromResult(entity);
    }
}
