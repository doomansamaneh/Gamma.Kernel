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

    public static string EscapeIdentifier(ISqlDialect dialect, string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return identifier;

        int spaceIndex = identifier.IndexOf(' ');
        string identifierPart;
        string aliasPart = "";

        if (spaceIndex < 0)
        {
            identifierPart = identifier;
        }
        else
        {
            identifierPart = identifier[..spaceIndex];
            aliasPart = identifier[(spaceIndex + 1)..].Trim();
        }

        var parts = identifierPart.Split('.');
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i].Trim();
            if (!part.StartsWith(dialect.EscapeStartChar) && !part.EndsWith(dialect.EscapeEndChar))
                parts[i] = $"{dialect.EscapeStartChar}{part}{dialect.EscapeEndChar}";
            else parts[i] = part;
        }

        var escapedIdentifier = string.Join(".", parts);

        if (string.IsNullOrEmpty(aliasPart))
            return escapedIdentifier;

        return $"{escapedIdentifier} {aliasPart}";
    }
}
