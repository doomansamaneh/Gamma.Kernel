using Dapper;
using FluentValidation;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Models;
using Gamma.Kernel.Security;
using Gamma.Kernel.Services;
using Gamma.Next.Application.Commands.Person;
using Gamma.Next.Application.Interfaces;

namespace Gamma.Next.Application.Services;

internal class PersonCommandService(
        IAuthorizationService authorizationService,
        ITransactionExecutor transactionExecutor,
        IDbConnectionFactory connectionFactory,
        ICommandHandler<AddPersonCommand, Guid> addHandler,
        ICommandHandler<EditPersonCommand, int> editHandler,
        ICommandHandler<DeletePersonCommand, int> deleteHandler,
        IValidator<AddPersonCommand> addValidator)
    : TransactionalServiceBase(authorizationService, transactionExecutor),
    IPersonCommandService
{

    [RequiresPermission("crm.person.edit")]
    public async Task<Result<int>> EditAsync(EditPersonCommand command, CancellationToken ct = default)
        => await ExecuteHandlerAsync(uow => editHandler.Handle(uow, command, ct), ct: ct);

    [RequiresPermission("crm.person.delete")]
    public async Task<Result<int>> DeleteAsync(DeletePersonCommand command, CancellationToken ct = default)
        => await ExecuteHandlerAsync(uow => deleteHandler.Handle(uow, command, ct), ct: ct);


    [RequiresPermission("crm.person.add")]
    public async Task<Result<Guid>> AddAsync(AddPersonCommand command, CancellationToken ct = default)
    {
        var validationResult = await ValidateAsync(command, ct);
        if (!validationResult.Success)
            return Result<Guid>.Fail(validationResult.Errors, validationResult.Message);

        return await ExecuteHandlerAsync(uow => addHandler.Handle(uow, command, ct), ct: ct);
    }

    private async Task<Result<bool>> ValidateAsync(AddPersonCommand command, CancellationToken ct = default)
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
                       .From("Crm.Person")
                       .Where("NationalCode = @NationalCode");

        var exists = await db.ExecuteScalarAsync<int>(query.ToString(), new { command.Person.NationalCode });

        if (exists > 0)
            return Result<bool>.Fail($"National Code '{command.Person.NationalCode}' already exists.");

        return Result<bool>.Ok(true);
    }
}