using FluentValidation;

namespace Gamma.Next.Application.Commands.Product;

public class ProductInputValidator : AbstractValidator<ProductInput>
{
    public ProductInputValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(3, 10).WithMessage("Code must be between 3 and 10 characters");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(3, 50).WithMessage("Title must be between 3 and 50 characters");

        RuleFor(x => x.Comment)
            .MaximumLength(200).WithMessage("Comment max length is 200");
    }
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product)
            .NotNull().WithMessage("Product must be provided")
            .SetValidator(new ProductInputValidator());
    }
}

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required for edit");

        RuleFor(x => x.Product)
            .NotNull().WithMessage("Product must be provided")
            .SetValidator(new ProductInputValidator());
    }
}