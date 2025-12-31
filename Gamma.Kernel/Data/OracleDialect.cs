using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public class OracleDialect : ISqlDialect
{
    public string EscapeIdentifier(string identifier) => $"\"{identifier}\"";

    public string ApplyPaging(
        string baseSql,
        string orderBy,
        string offsetParameter,
        string pageSizeParameter)
    {
        // Oracle uses OFFSET ... FETCH (11g+) or ROW_NUMBER
        return $"""
            {baseSql}
            ORDER BY {orderBy}
            OFFSET @{offsetParameter} ROWS
            FETCH NEXT @{pageSizeParameter} ROWS ONLY
            """;
    }

    public string GetLastInsertIdSql() => "SELECT seq_id.NEXTVAL FROM dual";

    public int DefaultCommandTimeout => 30;
    public string DatabaseProvider => "Oracle";
}