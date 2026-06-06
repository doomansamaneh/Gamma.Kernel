using FluentValidation;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using Gamma.Kernel.Abstractions;
using {{DomainNamespace}}.Interfaces.{{Schema}};

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Commands;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.update")]
public sealed record Update{{Entity}}Command(
{{UpdateCommandParameters}},
    long RowVersion
) : UpdateCommandBase<Domain.Entities.{{Schema}}.{{Entity}}>,
    IAuditableMessage, 
    I{{Entity}}Command;

public sealed class Update{{Entity}}CommandValidator : AbstractValidator<Update{{Entity}}Command>
{
    public Update{{Entity}}CommandValidator()
    {
        Include(new {{Entity}}SharedValidator<Update{{Entity}}Command>());
    }
}

public sealed class Update{{Entity}}CommandHandler(I{{Entity}}Repository repository)
    : UpdateCommandHandlerBase<Update{{Entity}}Command, Domain.Entities.{{Schema}}.{{Entity}}>(repository)
{
    protected override ValueTask UpdateEntity(
        Update{{Entity}}Command command,
        Domain.Entities.{{Schema}}.{{Entity}} entity,
        CancellationToken cancellationToken)
    {
        entity.Update(
{{UpdateNamedParameters}},
            rowVersion: command.RowVersion
        );

        return ValueTask.CompletedTask;
    }
}
