using FluentValidation;
using Gamma.Kernel.Common;

namespace Gamma.Next.Application.Ast.ProductGroup.Commands;

public interface IProductGroupCommand
{
    long RowNo { get; }
    string Code { get; }
    string Title { get; }
    string? Comment { get; }
    bool IsActive { get; }
}

public class ProductGroupSharedValidator<T> : AbstractValidator<T>
    where T : IProductGroupCommand
{
    public ProductGroupSharedValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithErrorCode(ValidationCodes.Required)
            .MaximumLength(240)
            .WithErrorCode(ValidationCodes.MaxLength);

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithErrorCode(ValidationCodes.Required)
            .MaximumLength(240)
            .WithErrorCode(ValidationCodes.MaxLength);

        RuleFor(x => x.Comment)
            .MaximumLength(240)
            .WithErrorCode(ValidationCodes.MaxLength);
    }
}
