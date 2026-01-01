using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Next.Application.ProductGroup.Dtos;

namespace Gamma.Next.Application.ProductGroup.Queries;

internal sealed class GetProductGroupQueryHandler(IDbConnectionFactory connectionFactory)
        : IQueryHandler<GetProductGroupQuery, PagedResult<ProductGroupDto>>
{
    public async Task<PagedResult<ProductGroupDto>> HandleAsync(
        GetProductGroupQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page;

        var sql = SQL.Select("pg.Id, pg.Code, pg.Title, pg.IsActive")
                    .From("ast.ProductGroup pg");
        using var _connection = connectionFactory.CreateConnection();

        // ---------- SEARCH (typed) ----------
        if (page.Search is not null)
        {
            if (!string.IsNullOrWhiteSpace(page.Search.Code))
            {
                sql.Where("pg.Code LIKE @Code");
                sql.AddParameter("Code", $"%{page.Search.Code}%");
            }

            if (!string.IsNullOrWhiteSpace(page.Search.Title))
            {
                sql.Where("pg.Title LIKE @Title");
                sql.AddParameter("Title", $"%{page.Search.Title}%");
            }

            if (page.Search.IsActive.HasValue)
            {
                sql.Where("pg.IsActive = @IsActive");
                sql.AddParameter("IsActive", page.Search.IsActive);
            }
        }

        // ---------- SEARCH TERM (free text) ----------
        if (!string.IsNullOrWhiteSpace(page.SearchTerm))
        {
            sql.Where("""
                (
                    pg.Code LIKE @SearchTerm OR
                    pg.Title LIKE @SearchTerm
                )
            """);

            sql.AddParameter("SearchTerm", $"%{page.SearchTerm}%");
        }

        // ---------- PAGED QUERY ----------
        return await _connection.QueryPagedAsync<ProductGroupDto>(
            sql,
            page,
            cancellationToken: cancellationToken
        );
    }
}
