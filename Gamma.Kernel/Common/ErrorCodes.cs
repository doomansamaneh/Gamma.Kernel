namespace Gamma.Kernel.Common;

public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";

    public const string EntityNotFound = "ENTITY_NOT_FOUND";
    public const string EntityAlreadyExists = "ENTITY_ALREADY_EXISTS";
    public const string EntityProtected = "ENTITY_PROTECTED";
    public const string EntityInUse = "ENTITY_IN_USE";

    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";

    public const string ConcurrencyConflict = "CONCURRENCY_CONFLICT";

    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";

    public const string OperationFailed = "OPERATION_FAILED";
}
