using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public class MySqlDialect : ISqlDialect
{
    public string WithNoLock => "";
    public char EscapeStartChar => '`';
    public char EscapeEndChar => '`';

    public string EscapeIdentifier(string identifier)
    {
        return SqlDialectResolver.EscapeIdentifier(this, identifier);
    }

    public string ApplyPaging(
        string baseSql,
        string orderBy,
        string offsetParameter,
        string pageSizeParameter)
    {
        // MySQL uses LIMIT offset, count
        return $"""
            {baseSql}
            ORDER BY {orderBy}
            LIMIT @{offsetParameter}, @{pageSizeParameter}
            """;
    }

    public string GetLastInsertIdSql() => "SELECT LAST_INSERT_ID()";

    public int DefaultCommandTimeout => 30;
    public string DatabaseProvider => "MySQL";
}