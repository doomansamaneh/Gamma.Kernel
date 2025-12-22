namespace Gamma.Kernel.Enums;

public enum ChangeAction
{
    Insert = 1,
    Update = 2,
}

public enum SanitizerType
{
    None = -1,
    HtmlFragment = 0,
    Html = 1,
    Xml = 2,
    URL = 3,
    XmlAttribute = 4,
    Css = 5,
    HtmlFormUrl = 6
}

public enum SqlClause
{
    DeleteFrom = 1,
    Update = 2,
    Set = 3,
    Select = 10,
    From = 11,
    Join = 12,
    Where = 13,
    GroupBy = 14,
    Having = 15,
    OrderBy = 16
}

public enum ErrorCodes
{
    // ---------- General ----------
    Unknown = 1000,
    InternalServerError = 1001,

    // ---------- Authorization ----------
    Unauthorized = 1100,
    Forbidden = 1101,

    // ---------- Validation ----------
    ValidationError = 1200,
    BusinessRuleViolation = 1201,

    // ---------- Database ----------
    DatabaseError = 1300,
    DatabaseForeignKey = 1301,
    DatabaseUniqueKey = 1302,
    DatabaseDataValidation = 1303,
    DatabaseTimeout = 1304,

    // ---------- Concurrency ----------
    ConcurrencyConflict = 1400,

    // ---------- Infrastructure ----------
    DependencyFailure = 1500,
    ServiceUnavailable = 1501
}

public enum DatabaseErrorType
{
    Unknown,
    ForeignKeyViolation,
    UniqueConstraintViolation,
    DataValidation,
    Timeout,
    ConcurrencyConflict
}

public enum AuditAction
{
    Create,
    Update,
    Delete,
    Read
}
