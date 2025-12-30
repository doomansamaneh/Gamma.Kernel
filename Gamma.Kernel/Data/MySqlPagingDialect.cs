using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public sealed class MySqlPagingDialect : ISqlPagingDialect
{
    public string ApplyPaging(
        string baseSql,
        string orderBy,
        string offsetParameter,
        string pageSizeParameter)
    {
        return $"""
            {baseSql}
            ORDER BY {orderBy}
            LIMIT @{offsetParameter}, @{pageSizeParameter}
            """;
    }
}