using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using Gamma.Next.Application.Ast.ProductGroup.Dtos;
using Gamma.Next.Application.Ast.ProductGroup.Sql;

namespace Gamma.Next.Application.Ast.ProductGroup.Queries;

[RequiresPermission("ast.productgroup.export")]
public sealed record ProductGroupExportQuery(
    PageFilterModel<ProductGroupSearchDto> Filter
) : IQuery<IEnumerable<ProductGroupGridDto>>;

internal sealed class ProductGroupExportQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<ProductGroupExportQuery, IEnumerable<ProductGroupGridDto>>
{
    public async ValueTask<IEnumerable<ProductGroupGridDto>> Handle(
        ProductGroupExportQuery query,
        CancellationToken ct)
    {
        var sql = ProductGroupSql.BuildExportQuery(query.Filter);

        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<ProductGroupGridDto>(
            sql,
            cancellationToken: ct
        );
    }
}
