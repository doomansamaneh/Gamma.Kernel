using Gamma.Kernel.Dapper;
using Gamma.Next.Application.ProductGroup.Dtos;

namespace Gamma.Next.Application.ProductGroup.Sql;

internal static class ProductGroupSql
{
    private const string TableName = "ast.ProductGroup";
    private const string TableAlias = "pg";

    internal static SqlBuilder BaseQuery()
    {
        return SQL.Select($"{TableAlias}.Id")
                  .Select($"{TableAlias}.Code")
                  .Select($"{TableAlias}.Title")
                  .Select($"{TableAlias}.IsActive")
                .From($"{TableName} {TableAlias}");
    }

    internal static SqlBuilder GetById()
    {
        var sql = BaseQuery()
                .Select($"{TableAlias}.RowVersion")
                .Where($"{TableAlias}.Id = @Id");
        return sql;
    }

    internal static SqlBuilder ApplySearch(this SqlBuilder sql, ProductGroupSearch? search)
    {
        if (search is null) return sql;

        if (!string.IsNullOrWhiteSpace(search.Code))
        {
            sql.Where($"{TableAlias}.Code LIKE @Code");
            sql.AddParameter("Code", $"%{search.Code}%");
        }

        if (!string.IsNullOrWhiteSpace(search.Title))
        {
            sql.Where($"{TableAlias}.Title LIKE @Title");
            sql.AddParameter("Title", $"%{search.Title}%");
        }

        if (search.IsActive.HasValue)
        {
            sql.Where($"{TableAlias}.IsActive = @IsActive");
            sql.AddParameter("IsActive", search.IsActive.Value);
        }

        return sql;
    }

    internal static SqlBuilder ApplySearchTerm(this SqlBuilder sql, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return sql;

        sql.Where($"""
            (
                {TableAlias}.Code LIKE @SearchTerm OR
                {TableAlias}.Title LIKE @SearchTerm
            )
        """);
        sql.AddParameter("SearchTerm", $"%{searchTerm}%");

        return sql;
    }
}
