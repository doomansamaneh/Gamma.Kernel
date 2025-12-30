namespace Gamma.Kernel.Abstractions;

public interface ISqlPagingDialect
{
    string ApplyPaging(
        string baseSql,
        string orderBy,
        string offsetParameter,
        string pageSizeParameter);
}