using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using Gamma.Next.Application.Ast.Product.Dtos;
using Gamma.Next.Application.Ast.Product.Sql;

namespace Gamma.Next.Application.Ast.Product.Queries;

[RequiresPermission("ast.product.read")]
[RequiresPermission("ast.product.create")]
public sealed record ProductGridQuery(
    PageModel<ProductSearchDto> Page
) : IQuery<PagedResult<ProductGridDto>>;

internal sealed class ProductQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<ProductGridQuery, PagedResult<ProductGridDto>>
{
    public async ValueTask<PagedResult<ProductGridDto>> Handle(
        ProductGridQuery query,
        CancellationToken ct)
    {
        var sql = ProductSql.BuildGridQuery(query.Page);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryPagedAsync<ProductGridDto>(
            sql,
            query.Page,
            cancellationToken: ct
        );
    }
}
