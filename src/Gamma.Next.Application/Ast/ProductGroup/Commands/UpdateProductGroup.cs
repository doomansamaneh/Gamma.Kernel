using FluentValidation;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.ProductGroup.Commands;

[RequiresPermission("ast.productgroup.update")]
public sealed record UpdateProductGroupCommand(
    long RowNo,
    string Code,
    string Title,
    string? Comment,
    bool IsActive,
    long RowVersion
) : UpdateCommandBase<Domain.Entities.Ast.ProductGroup>,
    IAuditableMessage, 
    IProductGroupCommand;

public sealed class UpdateProductGroupCommandValidator : AbstractValidator<UpdateProductGroupCommand>
{
    public UpdateProductGroupCommandValidator()
    {
        Include(new ProductGroupSharedValidator<UpdateProductGroupCommand>());
    }
}

public sealed class UpdateProductGroupCommandHandler(IProductGroupRepository repository)
    : UpdateCommandHandlerBase<UpdateProductGroupCommand, Domain.Entities.Ast.ProductGroup>(repository)
{
    protected override ValueTask UpdateEntity(
        UpdateProductGroupCommand command,
        Domain.Entities.Ast.ProductGroup entity,
        CancellationToken cancellationToken)
    {
        entity.Update(
            rowNo: command.RowNo,
            code: command.Code,
            title: command.Title,
            comment: command.Comment,
            isActive: command.IsActive,
            rowVersion: command.RowVersion
        );

        return ValueTask.CompletedTask;
    }
}
