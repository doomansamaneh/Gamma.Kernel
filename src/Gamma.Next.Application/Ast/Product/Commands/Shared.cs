using FluentValidation;
using Gamma.Kernel.Common;

namespace Gamma.Next.Application.Ast.Product.Commands;

public interface IProductCommand
{
    Guid ProductGroupId { get; }
    string Code { get; }
    string Title { get; }
    string? Comment { get; }
    bool IsActive { get; }
}

public class ProductSharedValidator<T> : AbstractValidator<T>
    where T : IProductCommand
{
    public ProductSharedValidator()
    {
        RuleFor(x => x.ProductGroupId)
            .NotEmpty()
            .WithErrorCode(ValidationCodes.Required);

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
