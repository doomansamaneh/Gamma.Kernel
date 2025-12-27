using FluentValidation;

namespace Gamma.Next.Application.Commands.Person;

public class AddPersonValidator : AbstractValidator<PersonInput>
{
    public AddPersonValidator()
    {
        RuleFor(x => x.NationalCode)
            .NotEmpty().WithMessage("National Code is required")
            .Length(10, 10).WithMessage("National Code must 10 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 50).WithMessage("Name must be between 3 and 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required")
            .Length(3, 50).WithMessage("LastName must be between 3 and 50 characters");
    }
}

public class AddPersonCommandValidator : AbstractValidator<AddPersonCommand>
{
    public AddPersonCommandValidator()
    {
        RuleFor(x => x.Person)
            .NotNull().WithMessage("Person must be provided")
            .SetValidator(new AddPersonValidator());
    }
}
