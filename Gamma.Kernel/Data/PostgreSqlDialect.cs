using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public class PostgreSqlDialect : ISqlDialect
{
    public string EscapeIdentifier(string identifier) => $"\"{identifier}\"";

    public string ApplyPaging(
        string baseSql,
        string orderBy,
        string offsetParameter,
        string pageSizeParameter)
    {
        // PostgreSQL uses LIMIT/OFFSET
        return $"""
            {baseSql}
            ORDER BY {orderBy}
            LIMIT @{pageSizeParameter} OFFSET @{offsetParameter}
            """;
    }

    public string GetLastInsertIdSql() => "SELECT lastval()";

    public int DefaultCommandTimeout => 30;
    public string DatabaseProvider => "PostgreSQL";
}
