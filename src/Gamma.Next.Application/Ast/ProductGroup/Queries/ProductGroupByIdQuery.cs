using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Queries;
using Gamma.Kernel.Security;
using Gamma.Next.Application.Ast.ProductGroup.Dtos;
using Gamma.Next.Application.Ast.ProductGroup.Sql;

namespace Gamma.Next.Application.Ast.ProductGroup.Queries;

[RequiresPermission("ast.productgroup.read")]
public sealed record ProductGroupByIdQuery(
    Guid Id
) : IGetByIdQuery<ProductGroupEditDto>;

internal sealed class ProductGroupByIdQueryHandler(
    IDbConnectionFactory connectionFactory
) : GetByIdQueryHandlerBase<ProductGroupByIdQuery, ProductGroupEditDto>(connectionFactory)
{
    protected override SqlBuilder Sql => ProductGroupSql.GetByIdQuery();
}
