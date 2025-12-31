using FluentValidation;

namespace Gamma.Next.Application.ProductGroup.Commands;

public class ProductGroupInputValidator : AbstractValidator<ProductGroupInput>
{
    public ProductGroupInputValidator()
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

public class CreateProductGroupCommandValidator : AbstractValidator<CreateProductGroupCommand>
{
    public CreateProductGroupCommandValidator()
    {
        RuleFor(x => x.ProductGroup)
            .NotNull().WithMessage("ProductGroup must be provided")
            .SetValidator(new ProductGroupInputValidator());
    }
}

public class UpdateProductGroupCommandValidator : AbstractValidator<UpdateProductGroupCommand>
{
    public UpdateProductGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required for edit");

        RuleFor(x => x.ProductGroup)
            .NotNull().WithMessage("ProductGroup must be provided")
            .SetValidator(new ProductGroupInputValidator());
    }
}
