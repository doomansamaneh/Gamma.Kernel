using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Security;
using {{DomainNamespace}}.Interfaces.{{Schema}};

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Commands;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.delete")]
public sealed record Delete{{Entity}}Command
    : DeleteCommandBase,
    IAuditableMessage;

internal sealed class Delete{{Entity}}CommandHandler(I{{Entity}}Repository repository
    ) : DeleteCommandHandlerBase<Delete{{Entity}}Command, Domain.Entities.{{Schema}}.{{Entity}}>(repository);
