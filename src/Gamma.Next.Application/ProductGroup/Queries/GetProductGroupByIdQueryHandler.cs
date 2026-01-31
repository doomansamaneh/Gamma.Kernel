using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Queries;
using Gamma.Next.Application.ProductGroup.Dtos;
using Gamma.Next.Application.ProductGroup.Sql;

namespace Gamma.Next.Application.ProductGroup.Queries;

internal sealed class GetProductGroupByIdQueryHandler(
    IDbConnectionFactory connectionFactory
) : GetByIdQueryHandlerBase<GetProductGroupByIdQuery, ProductGroupDto>(connectionFactory)
{
    protected override SqlBuilder Sql => ProductGroupSql.GetById();
}
