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

internal class ProductGroupService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IDbConnectionFactory connectionFactory,
    ICommandHandler<AddProductGroupCommand, Guid> addHandler,
    ICommandHandler<EditProductGroupCommand, int> editHandler,
    ICommandHandler<DeleteProductGroupCommand, int> deleteHandler,
    IValidator<AddProductGroupCommand> addValidator)
    : BaseCommandService<
        ICommandHandler<AddProductGroupCommand, Guid>,
        AddProductGroupCommand,
        ICommandHandler<EditProductGroupCommand, int>,
        EditProductGroupCommand,
        ICommandHandler<DeleteProductGroupCommand, int>,
        DeleteProductGroupCommand>(unitOfWorkFactory, addHandler, editHandler, deleteHandler),
    IProductGroupService,
    IApplicationService
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    private readonly IValidator<AddProductGroupCommand> _addValidator = addValidator;

    [RequiresPermission("ast.product-group.create")]
    public override async Task<Result<Guid>> AddAsync(AddProductGroupCommand command, CancellationToken ct = default)
    {
        var validationResult = await ValidateAsync(command, ct);
        if (!validationResult.Success)
            return Result<Guid>.Fail(validationResult.Errors, validationResult.Message);

        return await ExecuteHandlerAsync(uow => _addHandler.Handle(uow, command, ct), ct);
    }

    private async Task<Result<bool>> ValidateAsync(AddProductGroupCommand command, CancellationToken ct = default)
    {
        // FluentValidation
        var result = await _addValidator.ValidateAsync(command, ct);
        if (!result.IsValid)
            return Result<bool>.Fail(result.Errors.Select(e => e.ErrorMessage));

        // Database check
        using var db = _connectionFactory.CreateConnection();
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