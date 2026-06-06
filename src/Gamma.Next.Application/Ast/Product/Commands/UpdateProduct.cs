using FluentValidation;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.Product.Commands;

[RequiresPermission("ast.product.update")]
public sealed record UpdateProductCommand(
    Guid ProductGroupId,
    string Code,
    string Title,
    string? Comment,
    bool IsActive,
    long RowVersion
) : UpdateCommandBase<Domain.Entities.Ast.Product>,
    IAuditableMessage, 
    IProductCommand;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        Include(new ProductSharedValidator<UpdateProductCommand>());
    }
}

public sealed class UpdateProductCommandHandler(IProductRepository repository)
    : UpdateCommandHandlerBase<UpdateProductCommand, Domain.Entities.Ast.Product>(repository)
{
    protected override ValueTask UpdateEntity(
        UpdateProductCommand command,
        Domain.Entities.Ast.Product entity,
        CancellationToken cancellationToken)
    {
        entity.Update(
            productGroupId: command.ProductGroupId,
            code: command.Code,
            title: command.Title,
            comment: command.Comment,
            isActive: command.IsActive,
            rowVersion: command.RowVersion
        );

        return ValueTask.CompletedTask;
    }
}
