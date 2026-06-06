using FluentValidation;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using {{DomainNamespace}}.Interfaces.{{Schema}};

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Commands;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.create")]
public sealed record Create{{Entity}}Command(
{{CreateCommandParameters}}
) : CreateCommandBase<Domain.Entities.{{Schema}}.{{Entity}}>,
    IAuditableMessage,
    I{{Entity}}Command;

public class Create{{Entity}}CommandValidator : AbstractValidator<Create{{Entity}}Command>
{
    public Create{{Entity}}CommandValidator()
    {
        Include(new {{Entity}}SharedValidator<Create{{Entity}}Command>());
    }
}

public sealed class Create{{Entity}}CommandHandler(I{{Entity}}Repository repository)
    : CreateCommandHandlerBase<Create{{Entity}}Command, Domain.Entities.{{Schema}}.{{Entity}}>(repository)
{
    protected override ValueTask<Domain.Entities.{{Schema}}.{{Entity}}> CreateEntity(
        Create{{Entity}}Command command, 
        CancellationToken ct)
    {
        var entity = Domain.Entities.{{Schema}}.{{Entity}}.Create(
{{CreateNamedParameters}}
        );

        return ValueTask.FromResult(entity);
    }
}
