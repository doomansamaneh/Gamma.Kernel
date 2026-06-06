using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Queries;
using Gamma.Kernel.Security;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Sql;

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Queries;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.read")]
public sealed record {{Entity}}ByIdQuery(
    Guid Id
) : IGetByIdQuery<{{Entity}}EditDto>;

internal sealed class {{Entity}}ByIdQueryHandler(
    IDbConnectionFactory connectionFactory
) : GetByIdQueryHandlerBase<{{Entity}}ByIdQuery, {{Entity}}EditDto>(connectionFactory)
{
    protected override SqlBuilder Sql => {{Entity}}Sql.GetByIdQuery();
}
