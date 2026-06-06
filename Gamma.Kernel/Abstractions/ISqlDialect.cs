namespace Gamma.Kernel.Abstractions;

public interface ISqlDialect
{
    // ===== IDENTIFIER ESCAPING =====
    string WithNoLock { get; }
    char EscapeStartChar { get; }
    char EscapeEndChar { get; }
    string EscapeIdentifier(string identifier);
    string EscapeSql(string sql);

    // ===== PAGING =====
    string ApplyPaging(
        string baseSql,
        string orderBy,
        string offsetParameter,
        string pageSizeParameter);

    // ===== IDENTITY/SEQUENCE =====
    string GetLastInsertIdSql();

    // ===== CONFIGURATION =====
    int DefaultCommandTimeout { get; }
    string DatabaseProvider { get; }
}