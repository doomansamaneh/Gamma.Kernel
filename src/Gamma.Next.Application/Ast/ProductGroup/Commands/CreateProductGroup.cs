using FluentValidation;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using Gamma.Next.Domain.Interfaces.Ast;

namespace Gamma.Next.Application.Ast.ProductGroup.Commands;

[RequiresPermission("ast.productgroup.create")]
public sealed record CreateProductGroupCommand(
    long RowNo,
    string Code,
    string Title,
    string? Comment,
    bool IsActive
) : CreateCommandBase<Domain.Entities.Ast.ProductGroup>,
    IAuditableMessage,
    IProductGroupCommand;

public class CreateProductGroupCommandValidator : AbstractValidator<CreateProductGroupCommand>
{
    public CreateProductGroupCommandValidator()
    {
        Include(new ProductGroupSharedValidator<CreateProductGroupCommand>());
    }
}

public sealed class CreateProductGroupCommandHandler(IProductGroupRepository repository)
    : CreateCommandHandlerBase<CreateProductGroupCommand, Domain.Entities.Ast.ProductGroup>(repository)
{
    protected override ValueTask<Domain.Entities.Ast.ProductGroup> CreateEntity(
        CreateProductGroupCommand command, 
        CancellationToken ct)
    {
        var entity = Domain.Entities.Ast.ProductGroup.Create(
            rowNo: command.RowNo,
            code: command.Code,
            title: command.Title,
            comment: command.Comment,
            isActive: command.IsActive
        );

        return ValueTask.FromResult(entity);
    }
}
