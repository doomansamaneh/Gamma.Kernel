using Dapper;
using System.Data;
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

        // ---------- COUNT ----------
        var countBuilder = builder.Clone();
        var countSql = $"SELECT COUNT_BIG(1) {countBuilder.WithoutSelectAndOrderBy(dialect)}";

        var orderByClause = builder.GetOrderBy(dialect);
        if (string.IsNullOrWhiteSpace(orderByClause)) orderByClause = dialect.EscapeIdentifier("Id");

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
            builder.WithoutOrderBy(dialect),
            orderByClause,
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

    public static async Task<IEnumerable<T>> QueryAsync<T>(
        this IDbConnection connection,
        SqlBuilder builder,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(builder);

        var dialect = SqlDialectResolver.Resolve(connection);

        var dataSql = builder.ToSqlString(dialect);
        var items = await connection.QueryAsync<T>(
            new CommandDefinition(
                dataSql.NormalizeWhiteSpace(),
                builder.Parameters,
                transaction,
                commandTimeout,
                cancellationToken: cancellationToken)
        );

        return items;
    }

    public static async Task<T?> QueryFirstOrDefaultAsync<T>(
        this IDbConnection connection,
        SqlBuilder builder,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(builder);

        var dialect = SqlDialectResolver.Resolve(connection);

        var dataSql = builder.ToSqlString(dialect);
        return await connection.QueryFirstOrDefaultAsync<T>(
            new CommandDefinition(
                dataSql.NormalizeWhiteSpace(),
                builder.Parameters,
                transaction,
                commandTimeout,
                cancellationToken: cancellationToken)
        );
    }
}
