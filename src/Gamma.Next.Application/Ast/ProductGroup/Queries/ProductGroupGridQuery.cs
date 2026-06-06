using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using Gamma.Next.Application.Ast.ProductGroup.Dtos;
using Gamma.Next.Application.Ast.ProductGroup.Sql;

namespace Gamma.Next.Application.Ast.ProductGroup.Queries;

[RequiresPermission("ast.productgroup.read")]
[RequiresPermission("ast.productgroup.create")]
public sealed record ProductGroupGridQuery(
    PageModel<ProductGroupSearchDto> Page
) : IQuery<PagedResult<ProductGroupGridDto>>;

internal sealed class ProductGroupQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<ProductGroupGridQuery, PagedResult<ProductGroupGridDto>>
{
    public async ValueTask<PagedResult<ProductGroupGridDto>> Handle(
        ProductGroupGridQuery query,
        CancellationToken ct)
    {
        var sql = ProductGroupSql.BuildGridQuery(query.Page);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryPagedAsync<ProductGroupGridDto>(
            sql,
            query.Page,
            cancellationToken: ct
        );
    }
}
