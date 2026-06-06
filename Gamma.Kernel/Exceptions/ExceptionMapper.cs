using Gamma.Kernel.Enums;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Exceptions;

public static class ExceptionMapper
{
    public static ErrorResponse Map(Exception exception)
    {
        return exception switch
        {
            DatabaseException db => MapDatabase(db),

            GammaException ge => new ErrorResponse
            {
                Code = ge.StatusCode,
                Message = ge.Message
            },

            UnauthorizedAccessException => new ErrorResponse
            {
                Code = (int)ErrorCategory.Unauthorized,
                Message = "Unauthorized"
            },

            _ => new ErrorResponse
            {
                Code = (int)ErrorCategory.InternalServerError,
                Message = exception.Message
            }
        };
    }

    private static ErrorResponse MapDatabase(DatabaseException ex)
    {
        return ex.ErrorType switch
        {
            DatabaseErrorType.ForeignKeyViolation => new ErrorResponse
            {
                Code = (int)ErrorCategory.DatabaseForeignKey,
                Message = "Database foreign key violation"
            },

            DatabaseErrorType.UniqueConstraintViolation => new ErrorResponse
            {
                Code = (int)ErrorCategory.DatabaseUniqueKey,
                Message = "Duplicate value"
            },

            DatabaseErrorType.Timeout => new ErrorResponse
            {
                Code = (int)ErrorCategory.DatabaseTimeout,
                Message = "Database timeout"
            },

            DatabaseErrorType.DataValidation => new ErrorResponse
            {
                Code = (int)ErrorCategory.DatabaseDataValidation,
                Message = "Database validation error"
            },

            DatabaseErrorType.ConcurrencyConflict => new ErrorResponse
            {
                Code = (int)ErrorCategory.ConcurrencyConflict,
                Message = "Concurrency conflict"
            },

            _ => new ErrorResponse
            {
                Code = (int)ErrorCategory.InternalServerError,
                Message = "Database error"
            }
        };
    }
}
