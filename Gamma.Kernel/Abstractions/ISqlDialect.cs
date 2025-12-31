namespace Gamma.Kernel.Abstractions;

public interface ISqlDialect
{
    // ===== IDENTIFIER ESCAPING =====
    string EscapeIdentifier(string identifier);

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