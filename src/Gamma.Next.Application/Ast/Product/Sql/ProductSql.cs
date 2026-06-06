using Gamma.Kernel.Dapper;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using Gamma.Next.Application.Ast.Product.Dtos;
using Gamma.Next.Application.Ast.ProductGroup.Sql;

namespace Gamma.Next.Application.Ast.Product.Sql;

internal sealed class ProductSql : BaseSql<Domain.Entities.Ast.Product>
{
    private static readonly IReadOnlyDictionary<string, string> ColumnMapping
        = new Dictionary<string, string>(AutoColumnMapping, StringComparer.OrdinalIgnoreCase)
        {
            [nameof(ProductGridDto.ProductGroupTitle)] = ProductGroupSql.Column(c => c.Title),
        };

    private static SqlBuilder ApplyProductGroupJoin(SqlBuilder sql)
    {
        return sql.InnerJoin<Domain.Entities.Ast.Product, Domain.Entities.Ast.ProductGroup>(l => l.ProductGroupId, r => r.Id);
    }

    private static SqlBuilder LookupQuery()
    {
        var sql = SQL.Instance
            .Select<Domain.Entities.Ast.Product>(p => p.Id)
            .Select<Domain.Entities.Ast.Product>(p => p.Code)
            .Select<Domain.Entities.Ast.Product>(p => p.Title)
            .Select<Domain.Entities.Ast.ProductGroup>(pg => pg.Title, nameof(ProductLookupDto.ProductGroupTitle))
            .From<Domain.Entities.Ast.Product>();

        return ApplyProductGroupJoin(sql);
    }

    private static SqlBuilder GridQuery()
    {
        return LookupQuery()
            .Select<Domain.Entities.Ast.Product>(p => p.IsActive)
            .Select<Domain.Entities.Ast.Product>(p => p.Comment);
    }

    internal static SqlBuilder GetByIdQuery()
    {
        var sql = BaseSelect()
            .Select<Domain.Entities.Ast.ProductGroup>(pg => pg.Title, nameof(ProductEditDto.ProductGroupTitle));

        return ApplyProductGroupJoin(sql)
            .Where<Domain.Entities.Ast.Product, Guid>(x => x.Id, SqlOperator.Equals, "@Id");
    }

    internal static SqlBuilder BuildGridQuery(PageModel<ProductSearchDto> page)
    {
        var sql = GridQuery();

        ApplySearch(sql, page.Search);
        ApplySearchTerm(sql, page.SearchTerm);

        return sql.ApplySort(page.SortBy, page.SortOrder, ColumnMapping)
            .ApplyFilters(page.Filters, ColumnMapping);
    }

    internal static SqlBuilder BuildLookupQuery(PageModel<ProductSearchDto> page)
    {
        var sql = LookupQuery();

        ApplyLookup(sql);
        ApplySearchTerm(sql, page.SearchTerm);

        return sql.ApplySort(page.SortBy, page.SortOrder, ColumnMapping)
            .ApplyFilters(page.Filters, ColumnMapping);
    }

    internal static SqlBuilder BuildExportQuery(PageFilterModel<ProductSearchDto> filter)
    {
        var sql = GridQuery();

        ApplySearch(sql, filter.Search);
        ApplySearchTerm(sql, filter.SearchTerm);

        return sql.ApplySort(filter.SortBy, filter.SortOrder, ColumnMapping)
            .ApplyFilters(filter.Filters, ColumnMapping);
    }

    private static SqlBuilder ApplySearch(SqlBuilder sql, ProductSearchDto? search)
    {
        if (search is null) return sql;

        // TODO: add advanced search if needed

        return sql;
    }

    private static SqlBuilder ApplySearchTerm(SqlBuilder sql, string? searchTerm)
    {
        sql.ApplyMultiWordSearch(searchTerm,
            $"{TableAlias}.[{nameof(Domain.Entities.Ast.Product.Code)}]",
            $"{TableAlias}.[{nameof(Domain.Entities.Ast.Product.Title)}]");

        return sql;
    }

    private static SqlBuilder ApplyLookup(SqlBuilder sql)
    {
        sql.Where<Domain.Entities.Ast.Product, bool>(x => x.IsActive, SqlOperator.Equals, "@isActive");
        sql.AddParameter("isActive", true);

        return sql;
    }
}
