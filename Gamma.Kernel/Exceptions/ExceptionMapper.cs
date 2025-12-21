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
                Code = (int)ErrorCodes.Unauthorized,
                Message = "Unauthorized"
            },

            _ => new ErrorResponse
            {
                Code = (int)ErrorCodes.InternalServerError,
                Message = "An unexpected error occurred"
            }
        };
    }

    private static ErrorResponse MapDatabase(DatabaseException ex)
    {
        return ex.ErrorType switch
        {
            DatabaseErrorType.ForeignKeyViolation => new ErrorResponse
            {
                Code = (int)ErrorCodes.DatabaseForeignKey,
                Message = "Database foreign key violation"
            },

            DatabaseErrorType.UniqueConstraintViolation => new ErrorResponse
            {
                Code = (int)ErrorCodes.DatabaseUniqueKey,
                Message = "Duplicate value"
            },

            DatabaseErrorType.Timeout => new ErrorResponse
            {
                Code = (int)ErrorCodes.DatabaseTimeout,
                Message = "Database timeout"
            },

            DatabaseErrorType.DataValidation => new ErrorResponse
            {
                Code = (int)ErrorCodes.DatabaseDataValidation,
                Message = "Database validation error"
            },

            DatabaseErrorType.ConcurrencyConflict => new ErrorResponse
            {
                Code = (int)ErrorCodes.ConcurrencyConflict,
                Message = "Concurrency conflict"
            },

            _ => new ErrorResponse
            {
                Code = (int)ErrorCodes.InternalServerError,
                Message = "Database error"
            }
        };
    }
}
