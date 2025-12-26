using Dapper;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Models;
using Gamma.Kernel.Security;
using Gamma.Kernel.Services;
using Gamma.Next.Application.Commands.ProductGroup;
using Gamma.Next.Application.Interfaces;

namespace Gamma.Next.Application.Services;

internal class ProductGroupCommandService(
        IAuthorizationService authorizationService,
        ITransactionExecutor transactionExecutor,
        IDbConnectionFactory connectionFactory,
        ICommandHandler<AddProductGroupCommand, Guid> addHandler,
        ICommandHandler<EditProductGroupCommand, int> editHandler,
        ICommandHandler<DeleteProductGroupCommand, int> deleteHandler,
        IValidator<AddProductGroupCommand> addValidator)
    : TransactionalServiceBase(authorizationService, transactionExecutor),
    IProductGroupCommandService
{

    [RequiresPermission("ast.product-group.edit")]
    public async Task<Result<int>> EditAsync(EditProductGroupCommand command, CancellationToken ct = default)
        => await ExecuteHandlerAsync(uow => editHandler.Handle(uow, command, ct), ct: ct);

    [RequiresPermission("ast.product-group.delete")]
    public async Task<Result<int>> DeleteAsync(DeleteProductGroupCommand command, CancellationToken ct = default)
        => await ExecuteHandlerAsync(uow => deleteHandler.Handle(uow, command, ct), ct: ct);


    [RequiresPermission("ast.product-group.add")]
    public async Task<Result<Guid>> AddAsync(AddProductGroupCommand command, CancellationToken ct = default)
    {
        //await EnsurePermissionAsync("ast.product-group.create", ct);

        var validationResult = await ValidateAsync(command, ct);
        if (!validationResult.Success)
            return Result<Guid>.Fail(validationResult.Errors, validationResult.Message);

        return await ExecuteHandlerAsync(uow => addHandler.Handle(uow, command, ct), ct: ct);
    }

    private async Task<Result<bool>> ValidateAsync(AddProductGroupCommand command, CancellationToken ct = default)
    {
        //FluentValidation
        var result = await addValidator.ValidateAsync(command, ct);
        if (!result.IsValid)
            return Result<bool>.Fail(result.Errors.Select(e => e.ErrorMessage));

        // Database check
        using var db = connectionFactory.CreateConnection();
        if (db.State != System.Data.ConnectionState.Open)
            db.Open();

        var query = SQL.Select("count(1)")
                       .From("Ast.ProductGroup")
                       .Where("Code = @Code");

        var exists = await db.ExecuteScalarAsync<int>(query.ToString(), new { command.ProductGroup.Code });

        if (exists > 0)
            return Result<bool>.Fail($"Code '{command.ProductGroup.Code}' already exists.");

        return Result<bool>.Ok(true);
    }
}