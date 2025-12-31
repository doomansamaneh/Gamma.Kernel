using System.Data;
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public static class SqlDialectResolver
{
    public static ISqlDialect Resolve(IDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var name = connection.GetType().Name;
        return name switch
        {
            var n when n.Contains("SqlConnection") => new SqlServerDialect(),
            var n when n.Contains("Npgsql") => new PostgreSqlDialect(),
            var n when n.Contains("MySql") => new MySqlDialect(),
            var n when n.Contains("Oracle") => new OracleDialect(),
            _ => throw new NotSupportedException(
                $"SQL dialect not supported for connection type '{name}'. " +
                $"Supported databases: SQL Server, PostgreSQL, MySQL, Oracle.")
        };
    }
}
