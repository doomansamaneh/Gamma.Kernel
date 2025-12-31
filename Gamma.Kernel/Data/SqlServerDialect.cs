using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public class SqlServerDialect : ISqlDialect
{
    public string EscapeIdentifier(string identifier) => $"[{identifier}]";

    public string ApplyPaging(
        string baseSql,
        string orderBy,
        string offsetParameter,
        string pageSizeParameter)
    {
        // SQL Server uses OFFSET ... FETCH
        return $"""
            {baseSql}
            ORDER BY {orderBy}
            OFFSET @{offsetParameter} ROWS
            FETCH NEXT @{pageSizeParameter} ROWS ONLY
            """;
    }

    public string GetLastInsertIdSql() => "SELECT CAST(SCOPE_IDENTITY() as bigint)";

    public int DefaultCommandTimeout => 30;
    public string DatabaseProvider => "SqlServer";
}