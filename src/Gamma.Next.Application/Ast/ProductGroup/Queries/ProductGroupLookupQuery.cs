using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using Gamma.Next.Application.Ast.ProductGroup.Dtos;
using Gamma.Next.Application.Ast.ProductGroup.Sql;

namespace Gamma.Next.Application.Ast.ProductGroup.Queries;

[RequiresPermission("ast.productgroup.lookup")]
[RequiresPermission("ast.productgroup.read")]
[RequiresPermission("ast.productgroup.create")]
public sealed record ProductGroupLookupQuery(
    PageModel<ProductGroupSearchDto> Page
) : IQuery<PagedResult<ProductGroupLookupDto>>;

internal sealed class ProductGroupLookupQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<ProductGroupLookupQuery, PagedResult<ProductGroupLookupDto>>
{
    public async ValueTask<PagedResult<ProductGroupLookupDto>> Handle(
        ProductGroupLookupQuery query,
        CancellationToken ct)
    {
        var sql = ProductGroupSql.BuildLookupQuery(query.Page);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryPagedAsync<ProductGroupLookupDto>(
            sql,
            query.Page,
            cancellationToken: ct
        );
    }
}
