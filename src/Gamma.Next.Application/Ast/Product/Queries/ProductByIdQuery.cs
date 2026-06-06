using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Queries;
using Gamma.Kernel.Security;
using Gamma.Next.Application.Ast.Product.Dtos;
using Gamma.Next.Application.Ast.Product.Sql;

namespace Gamma.Next.Application.Ast.Product.Queries;

[RequiresPermission("ast.product.read")]
public sealed record ProductByIdQuery(
    Guid Id
) : IGetByIdQuery<ProductEditDto>;

internal sealed class ProductByIdQueryHandler(
    IDbConnectionFactory connectionFactory
) : GetByIdQueryHandlerBase<ProductByIdQuery, ProductEditDto>(connectionFactory)
{
    protected override SqlBuilder Sql => ProductSql.GetByIdQuery();
}
