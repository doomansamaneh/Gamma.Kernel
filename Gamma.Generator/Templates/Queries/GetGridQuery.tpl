using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Sql;

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Queries;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.read")]
[RequiresPermission("{{schema_lower}}.{{entity_lower}}.create")]
public sealed record {{Entity}}GridQuery(
    PageModel<{{Entity}}SearchDto> Page
) : IQuery<PagedResult<{{Entity}}GridDto>>;

internal sealed class {{Entity}}QueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<{{Entity}}GridQuery, PagedResult<{{Entity}}GridDto>>
{
    public async ValueTask<PagedResult<{{Entity}}GridDto>> Handle(
        {{Entity}}GridQuery query,
        CancellationToken ct)
    {
        var sql = {{Entity}}Sql.BuildGridQuery(query.Page);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryPagedAsync<{{Entity}}GridDto>(
            sql,
            query.Page,
            cancellationToken: ct
        );
    }
}
