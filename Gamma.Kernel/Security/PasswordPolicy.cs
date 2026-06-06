namespace Gamma.Kernel.Security;

public sealed class PasswordPolicy
{
    public int MinLength { get; set; } = 5;

    public int MaxLength { get; set; } = 50;

    public bool RequireUppercase { get; set; }

    public bool RequireLowercase { get; set; }

    public bool RequireDigit { get; set; }

    public bool RequireSpecialCharacter { get; set; }
}
