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

public enum ErrorCategory
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
    Create = 1,
    Update = 2,
    Delete = 3,
    Read = 4,
    Custom = 5
}

public enum SortOrder
{
    Ascending = 1,
    Descending = 2
}

public enum SqlOperator
{
    Equals = 1,
    NotEquals = 2,
    Contains = 3,
    NotContains = 4,
    LessThan = 5,
    LessThanOrEqual = 6,
    GreaterThan = 7,
    GreaterThanOrEqual = 8,
    In = 9,
    NotIn = 10,
    StartsWith = 11,
    EndsWith = 12,
    IsNull = 13,
    IsNotNull = 14,
    None = 99,
}