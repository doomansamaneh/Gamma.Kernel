using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Next.Application.ProductGroup.Dtos;
using Gamma.Next.Application.ProductGroup.Sql;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Queries;

internal sealed class GetProductGroupQueryHandler(IDbConnectionFactory connectionFactory)
        : IQueryHandler<GetProductGroupQuery, PagedResult<ProductGroupDto>>
{
    public async ValueTask<PagedResult<ProductGroupDto>> Handle(
        GetProductGroupQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page;

        var sql = ProductGroupSql.BaseQuery()
                             .ApplySearch(page.Search)
                             .ApplySearchTerm(page.SearchTerm);

        using var _connection = connectionFactory.CreateConnection();
        return await _connection.QueryPagedAsync<ProductGroupDto>(
            sql,
            page,
            cancellationToken: cancellationToken
        );
    }
}
