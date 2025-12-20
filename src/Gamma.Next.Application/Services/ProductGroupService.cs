using Dapper;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Models;
using Gamma.Next.Application.Commands.ProductGroup;
using Gamma.Next.Application.Commands.Shared;
using Gamma.Next.Application.Interfaces;

namespace Gamma.Next.Application.Services;

internal class ProductGroupService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IDbConnectionFactory connectionFactory,
    ICommandHandler<AddProductGroupCommand, Guid> addHandler,
    ICommandHandler<EditProductGroupCommand, int> editHandler,
    ICommandHandler<DeleteCommand, int> deleteHandler,
    IValidator<AddProductGroupCommand> addProductGroupValidator
) : ICommandService<AddProductGroupCommand, EditProductGroupCommand, Guid>
{
    private async Task<Result<T>> ExecuteHandlerAsync<T>(Func<Task<Result<T>>> handlerFunc)
    {
        await using var uow = unitOfWorkFactory.Create();
        var result = await handlerFunc();

        if (!result.Success) await uow.RollbackAsync();
        else await uow.CommitAsync();

        return result;
    }

    public async Task<Result<Guid>> AddAsync(AddProductGroupCommand command, CancellationToken ct = default)
    {
        var validation = await ValidateAsync(command, ct);
        if (!validation.Success) return Result<Guid>.Fail(validation.Errors, validation.Message);

        return await ExecuteHandlerAsync(() => addHandler.Handle(command, ct));
    }

    public Task<Result<int>> EditAsync(EditProductGroupCommand command, CancellationToken ct = default)
        => ExecuteHandlerAsync(() => editHandler.Handle(command, ct));

    public Task<Result<int>> DeleteAsync(DeleteCommand command, CancellationToken ct = default)
        => ExecuteHandlerAsync(() => deleteHandler.Handle(command, ct));

    private async Task<Result<bool>> ValidateAsync(AddProductGroupCommand command, CancellationToken ct = default)
    {
        var result = await addProductGroupValidator.ValidateAsync(command, ct);
        if (!result.IsValid)
            return Result<bool>.Fail(result.Errors.Select(e => e.ErrorMessage));

        using var db = connectionFactory.CreateConnection();
        if (db.State != System.Data.ConnectionState.Open) db.Open();
        // Duplicate Check
        var query = SQL.Select("count(1)")
                    .From("Ast.ProductGroup")
                    .Where("Code = @Code");
        var exists = await db.ExecuteScalarAsync<int>(query.ToString(), new { command.ProductGroup.Code });

        if (exists > 0)
            return Result<bool>.Fail($"Code '{command.ProductGroup.Code}' already exists.");

        return Result<bool>.Ok(true);
    }
}
