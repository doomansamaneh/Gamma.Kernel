using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using Gamma.Next.Application.Ast.Product.Dtos;
using Gamma.Next.Application.Ast.Product.Sql;

namespace Gamma.Next.Application.Ast.Product.Queries;

[RequiresPermission("ast.product.lookup")]
[RequiresPermission("ast.product.read")]
[RequiresPermission("ast.product.create")]
public sealed record ProductLookupQuery(
    PageModel<ProductSearchDto> Page
) : IQuery<PagedResult<ProductLookupDto>>;

internal sealed class ProductLookupQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<ProductLookupQuery, PagedResult<ProductLookupDto>>
{
    public async ValueTask<PagedResult<ProductLookupDto>> Handle(
        ProductLookupQuery query,
        CancellationToken ct)
    {
        var sql = ProductSql.BuildLookupQuery(query.Page);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryPagedAsync<ProductLookupDto>(
            sql,
            query.Page,
            cancellationToken: ct
        );
    }
}
