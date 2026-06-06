using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Sql;

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Queries;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.lookup")]
[RequiresPermission("{{schema_lower}}.{{entity_lower}}.read")]
[RequiresPermission("{{schema_lower}}.{{entity_lower}}.create")]
public sealed record {{Entity}}LookupQuery(
    PageModel<{{Entity}}SearchDto> Page
) : IQuery<PagedResult<{{Entity}}LookupDto>>;

internal sealed class {{Entity}}LookupQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<{{Entity}}LookupQuery, PagedResult<{{Entity}}LookupDto>>
{
    public async ValueTask<PagedResult<{{Entity}}LookupDto>> Handle(
        {{Entity}}LookupQuery query,
        CancellationToken ct)
    {
        var sql = {{Entity}}Sql.BuildLookupQuery(query.Page);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryPagedAsync<{{Entity}}LookupDto>(
            sql,
            query.Page,
            cancellationToken: ct
        );
    }
}
