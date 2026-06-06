using FluentValidation;

namespace Gamma.Kernel.Security;

public static class PasswordValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ApplyPasswordPolicy<T>(
        this IRuleBuilder<T, string> rule,
        PasswordPolicy policy)
    {
        var builder = rule
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(policy.MinLength)
            .WithMessage($"Password must be at least {policy.MinLength} characters long.")
            .MaximumLength(policy.MaxLength)
            .WithMessage($"Password cannot exceed {policy.MaxLength} characters.");

        if (policy.RequireUppercase)
        {
            builder = builder
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.");
        }

        if (policy.RequireLowercase)
        {
            builder = builder
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.");
        }

        if (policy.RequireDigit)
        {
            builder = builder
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.");
        }

        if (policy.RequireSpecialCharacter)
        {
            builder = builder
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");
        }

        return builder;
    }
}

