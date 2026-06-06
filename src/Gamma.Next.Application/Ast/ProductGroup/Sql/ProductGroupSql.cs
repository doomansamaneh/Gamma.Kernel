using Gamma.Kernel.Dapper;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Next.Application.Ast.ProductGroup.Dtos;

namespace Gamma.Next.Application.Ast.ProductGroup.Sql;

internal sealed class ProductGroupSql : BaseSql<Domain.Entities.Ast.ProductGroup>
{
    private static readonly IReadOnlyDictionary<string, string> ColumnMapping
        = new Dictionary<string, string>(AutoColumnMapping, StringComparer.OrdinalIgnoreCase);

    private static SqlBuilder LookupQuery()
    {
        return BaseSelect();
    }

    private static SqlBuilder GridQuery()
    {
        return LookupQuery();
    }

    internal static SqlBuilder GetByIdQuery()
    {
        return BaseSelect()
            .Where<Domain.Entities.Ast.ProductGroup, Guid>(x => x.Id, SqlOperator.Equals, "@Id");
    }

    internal static SqlBuilder BuildGridQuery(PageModel<ProductGroupSearchDto> page)
    {
        var sql = GridQuery();

        ApplySearch(sql, page.Search);
        ApplySearchTerm(sql, page.SearchTerm);

        return sql.ApplySort(page.SortBy, page.SortOrder, ColumnMapping)
                .ApplyFilters(page.Filters, ColumnMapping);
    }

    internal static SqlBuilder BuildLookupQuery(PageModel<ProductGroupSearchDto> page)
    {
        var sql = LookupQuery();

        ApplyLookup(sql);
        ApplySearchTerm(sql, page.SearchTerm);

        return sql.ApplySort(page.SortBy, page.SortOrder, ColumnMapping)
                .ApplyFilters(page.Filters, ColumnMapping);
    }

    internal static SqlBuilder BuildExportQuery(PageFilterModel<ProductGroupSearchDto> filter)
    {
        var sql = GridQuery();

        ApplySearch(sql, filter.Search);
        ApplySearchTerm(sql, filter.SearchTerm);

        return sql.ApplySort(filter.SortBy, filter.SortOrder, ColumnMapping)
                .ApplyFilters(filter.Filters, ColumnMapping);
    }

    private static SqlBuilder ApplySearch(SqlBuilder sql, ProductGroupSearchDto? search)
    {
        if (search is null) return sql;

        return sql;
    }

    private static SqlBuilder ApplySearchTerm(SqlBuilder sql, string? searchTerm)
    {
        sql.ApplyMultiWordSearch(searchTerm,
            $"{TableAlias}.[{nameof(Domain.Entities.Ast.ProductGroup.Code)}]",
            $"{TableAlias}.[{nameof(Domain.Entities.Ast.ProductGroup.Title)}]");
        return sql;
    }

    private static SqlBuilder ApplyLookup(SqlBuilder sql)
    {
        sql.Where<Domain.Entities.Ast.ProductGroup, bool>(x => x.IsActive, SqlOperator.Equals, "@isActive");
        sql.AddParameter("isActive", true);
        return sql;
    }
}
