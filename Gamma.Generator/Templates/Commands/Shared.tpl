using FluentValidation;
using Gamma.Kernel.Common;

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Commands;

public interface I{{Entity}}Command
{
{{InterfaceProperties}}
}

public class {{Entity}}SharedValidator<T> : AbstractValidator<T>
    where T : I{{Entity}}Command
{
    public {{Entity}}SharedValidator()
    {
    {{SharedValidatorRules}}
    }
}
