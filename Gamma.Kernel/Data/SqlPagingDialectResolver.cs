using System.Data;
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public static class SqlPagingDialectResolver
{
    public static ISqlPagingDialect Resolve(IDbConnection connection)
    {
        var name = connection.GetType().Name;

        return name switch
        {
            var n when n.Contains("SqlConnection") => new SqlServerPagingDialect(),
            var n when n.Contains("Npgsql") => new PostgreSqlPagingDialect(),
            var n when n.Contains("MySql") => new MySqlPagingDialect(),
            _ => throw new NotSupportedException(
                $"Paging not supported for connection type {name}")
        };
    }
}
