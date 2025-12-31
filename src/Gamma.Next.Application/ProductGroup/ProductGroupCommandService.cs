using Dapper;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Commands;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Models;
using Gamma.Kernel.Security;
using Gamma.Kernel.Services;
using Gamma.Next.Application.Commands.Product;
using Gamma.Next.Application.ProductGroup.Commands;

namespace Gamma.Next.Application.ProductGroup;

internal class ProductGroupCommandService(
        IAuthorizationService authorizationService,
        ITransactionExecutor transactionExecutor,
        IDbConnectionFactory connectionFactory,

        // todo: how to avoid constructor explosion? lazy loading?, resolver pattern?
        // Generic handlers
        ICreateCommandHandler<Domain.Entities.ProductGroup> addGroupHandler,
        IUpdateCommandHandler<Domain.Entities.ProductGroup> updateGroupHandler,
        IDeleteCommandHandler<Domain.Entities.ProductGroup, Guid> deleteGroupHandler,
        ICreateCommandHandler<Domain.Entities.Product> addProductHandler,
        IUpdateCommandHandler<Domain.Entities.Product> updateProductHandler,
        IDeleteCommandHandler<Domain.Entities.Product, Guid> deleteProductHandler,

        IValidator<CreateProductGroupCommand> createGroupValidator,
        IValidator<CreateProductCommand> createProductValidator
    )
    : TransactionalServiceBase(authorizationService, transactionExecutor),
      IProductGroupCommandService
{
    [RequiresPermission("ast.product-group.add")]
    public async Task<Result<Guid>> CreateAsync(CreateProductGroupCommand command, CancellationToken ct = default)
    {
        // 1️⃣ Validate ProductGroup
        var validationResult = await ValidateAsync(command, ct);
        if (!validationResult.Success)
            return Result<Guid>.Fail(validationResult.Errors, validationResult.Message);

        Guid productGroupId;

        // 2️⃣ Execute all handlers in a transaction
        var result = await ExecuteHandlerAsync(async uow =>
        {
            // 2a Add ProductGroup
            var createGroupCommand = new GenericCreateCommand<Domain.Entities.ProductGroup>(command.ProductGroup.ToEntity());
            var groupResult = await addGroupHandler.Handle(uow, createGroupCommand, ct);
            if (!groupResult.Success)
                return Result<Guid>.Fail(groupResult.Errors, groupResult.Message);

            productGroupId = groupResult.Data;

            // 2b Add each Product as detail
            foreach (var item in command.ProductGroup.Products)
            {
                var createProductCommand = new GenericCreateCommand<Domain.Entities.Product>(item.ToEntity(productGroupId));

                var productResult = await addProductHandler.Handle(uow, createProductCommand, ct);
                if (!productResult.Success)
                    return Result<Guid>.Fail(productResult.Errors, productResult.Message);
            }

            return Result<Guid>.Ok(productGroupId);
        }, ct);

        return result;
    }

    [RequiresPermission("ast.product-group.edit")]
    public async Task<Result<int>> UpdateAsync(UpdateProductGroupCommand command, CancellationToken ct = default)
    {
        var updateCommand = new GenericUpdateCommand<Domain.Entities.ProductGroup>(command.ProductGroup.ToEntity(command.Id));

        return await ExecuteHandlerAsync(uow => updateGroupHandler.Handle(uow, updateCommand, ct), ct: ct);
    }

    [RequiresPermission("ast.product-group.delete")]
    public async Task<Result<int>> DeleteAsync(DeleteProductGroupCommand command, CancellationToken ct = default)
    {
        var result = await ExecuteHandlerAsync(async uow =>
        {
            // Delete all products in this group
            // var deleteProductsCommand = new DeleteProductsByProductGroupCommand(command.Id);
            // var productResult = await deleteProductHandler.Handle(uow, deleteProductsCommand, ct);
            // if (!productResult.Success)
            //     return Result<int>.Fail(productResult.Errors, productResult.Message);

            // Delete ProductGroup
            var deleteGroupCommand = new GenericDeleteCommand<Domain.Entities.ProductGroup, Guid>(command.Id);
            var groupResult = await deleteGroupHandler.Handle(uow, deleteGroupCommand, ct);
            if (!groupResult.Success)
                return Result<int>.Fail(groupResult.Errors, groupResult.Message);

            return Result<int>.Ok(1);
        }, ct);

        return result;
    }

    private async Task<Result<bool>> ValidateAsync(CreateProductGroupCommand command, CancellationToken ct = default)
    {
        // 1️⃣ FluentValidation
        var validationResult = await createGroupValidator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
            return Result<bool>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        // 2️⃣ Database check for uniqueness
        using var db = connectionFactory.CreateConnection();
        var query = SQL.Select("count(1)")
                       .From("Ast.ProductGroup")
                       .Where("Code = @Code");

        var exists = await db.ExecuteScalarAsync<int>(query.ToString(), new { command.ProductGroup.Code });
        if (exists > 0)
            return Result<bool>.Fail($"Code '{command.ProductGroup.Code}' already exists.");

        return Result<bool>.Ok(true);
    }
}

//todo: use dynamic mapping tool like AutoMapper
internal static class ProductGroupCommandExtensions
{
    public static Domain.Entities.ProductGroup ToEntity(this ProductGroupInput input, Guid? id = null) =>
        new()
        {
            Id = id ?? Guid.Empty,
            Code = input.Code,
            Title = input.Title,
            Comment = input.Comment,
            IsActive = input.IsActive
        };

    public static Domain.Entities.Product ToEntity(this ProductInput input, Guid productGroupId, Guid? id = null) =>
        new()
        {
            Id = id ?? Guid.Empty,
            ProductGroupId = productGroupId,
            Code = input.Code,
            Title = input.Title,
            Comment = input.Comment,
            IsActive = input.IsActive
        };
}