using Dapper;
using System.Data;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Data;

namespace Gamma.Kernel.Extensions;

public static class DapperExtensions
{
    public static async Task<PagedResult<T>> QueryPagedAsync<T>(
        this IDbConnection connection,
        SqlBuilder builder,
        PageModel page,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(page);

        page.Normalize();
        var dialect = SqlDialectResolver.Resolve(connection);

        // ---------- ORDER BY ----------
        var orderBy = !string.IsNullOrWhiteSpace(page.SortBy)
            ? dialect.EscapeIdentifier(page.SortBy.ToSafeSqlField())
            : dialect.EscapeIdentifier("Id");

        if (page.SortOrder == SortOrder.Descending)
            orderBy += " DESC";

        // ---------- COUNT ----------
        var countBuilder = builder.Clone();
        var countSql = $"SELECT COUNT_BIG(1) {countBuilder.WithoutSelectAndOrderBy()}";

        var totalItems = await connection.ExecuteScalarAsync<long>(
            new CommandDefinition(
                countSql.NormalizeWhiteSpace(),
                countBuilder.Parameters,
                transaction,
                commandTimeout,
                cancellationToken: cancellationToken)
        );

        if (totalItems == 0)
        {
            return new PagedResult<T>
            {
                Items = [],
                TotalItems = 0,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }

        // ---------- DATA ----------
        var dataSql = dialect.ApplyPaging(
            builder.ToString(),
            orderBy,
            offsetParameter: "Offset",
            pageSizeParameter: "PageSize"
        );

        var parameters = new DynamicParameters(builder.Parameters);
        parameters.Add("Offset", page.Offset);
        parameters.Add("PageSize", page.PageSize);

        var items = (await connection.QueryAsync<T>(
            new CommandDefinition(
                dataSql.NormalizeWhiteSpace(),
                parameters,
                transaction,
                commandTimeout,
                cancellationToken: cancellationToken)
        )).AsList();

        return new PagedResult<T>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }
}
