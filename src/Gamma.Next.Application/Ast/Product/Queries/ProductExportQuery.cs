using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Security;
using Gamma.Zed.Application.Ast.Product.Dtos;
using Gamma.Zed.Application.Ast.Product.Sql;

namespace Gamma.Zed.Application.Ast.Product.Queries;

[RequiresPermission("ast.product.export")]
public sealed record ProductExportQuery(
    PageFilterModel<ProductSearchDto> Filter
) : IQuery<IEnumerable<ProductGridDto>>;

internal sealed class ProductExportQueryHandler(IDbConnectionFactory connectionFactory)
    : IQueryHandler<ProductExportQuery, IEnumerable<ProductGridDto>>
{
    public async ValueTask<IEnumerable<ProductGridDto>> Handle(
        ProductExportQuery query,
        CancellationToken ct)
    {
        var sql = ProductSql.BuildExportQuery(query.Filter);
        
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<ProductGridDto>(
            sql,
            cancellationToken: ct
        );
    }
}
