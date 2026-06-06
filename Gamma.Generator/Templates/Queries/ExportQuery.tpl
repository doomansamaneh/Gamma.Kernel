using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Sql;

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Queries;

[RequiresPermission("{{schema_lower}}.{{entity_lower}}.export")]
public sealed record {{Entity}}ExportQuery(
    PageFilterModel<{{Entity}}SearchDto> Filter
) : IQuery<IEnumerable<{{Entity}}GridDto>>;

internal sealed class {{Entity}}ExportQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<{{Entity}}ExportQuery, IEnumerable<{{Entity}}GridDto>>
{
    public async ValueTask<IEnumerable<{{Entity}}GridDto>> Handle(
        {{Entity}}ExportQuery query,
        CancellationToken ct)
    {
        var sql = {{Entity}}Sql.BuildExportQuery(query.Filter);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<{{Entity}}GridDto>(
            sql,
            cancellationToken: ct
        );
    }
}
