using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Data;

public sealed class SqlServerPagingDialect : ISqlPagingDialect
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
            OFFSET @{offsetParameter} ROWS
            FETCH NEXT @{pageSizeParameter} ROWS ONLY
            """;
    }
}
