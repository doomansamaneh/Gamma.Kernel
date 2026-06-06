using Gamma.Kernel.Dapper;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;
using Gamma.Kernel.Paging;
using {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;

namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Sql;

internal sealed class {{Entity}}Sql : BaseSql<Domain.Entities.{{Schema}}.{{Entity}}>
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
            .Where<Domain.Entities.{{Schema}}.{{Entity}}, Guid>(x => x.Id, SqlOperator.Equals, "@Id");
    }

    internal static SqlBuilder BuildGridQuery(PageModel<{{Entity}}SearchDto> page)
    {
        var sql = GridQuery();

        ApplySearch(sql, page.Search);
        ApplySearchTerm(sql, page.SearchTerm);

        return sql.ApplySort(page.SortBy, page.SortOrder, ColumnMapping)
                .ApplyFilters(page.Filters, ColumnMapping);
    }

    internal static SqlBuilder BuildLookupQuery(PageModel<{{Entity}}SearchDto> page)
    {
        var sql = LookupQuery();

        ApplyLookup(sql);
        ApplySearchTerm(sql, page.SearchTerm);

        return sql.ApplySort(page.SortBy, page.SortOrder, ColumnMapping)
                .ApplyFilters(page.Filters, ColumnMapping);
    }

    internal static SqlBuilder BuildExportQuery(PageFilterModel<{{Entity}}SearchDto> filter)
    {
        var sql = GridQuery();

        ApplySearch(sql, filter.Search);
        ApplySearchTerm(sql, filter.SearchTerm);

        return sql.ApplySort(filter.SortBy, filter.SortOrder, ColumnMapping)
                .ApplyFilters(filter.Filters, ColumnMapping);
    }

    private static SqlBuilder ApplySearch(SqlBuilder sql, {{Entity}}SearchDto? search)
    {
        if (search is null) return sql;

        return sql;
    }

    private static SqlBuilder ApplySearchTerm(SqlBuilder sql, string? searchTerm)
    {
        sql.ApplyMultiWordSearch(searchTerm,
{{SearchTermConditions}});
        return sql;
    }

    private static SqlBuilder ApplyLookup(SqlBuilder sql)
    {
    {{LookupCondition}}
        return sql;
    }
}
