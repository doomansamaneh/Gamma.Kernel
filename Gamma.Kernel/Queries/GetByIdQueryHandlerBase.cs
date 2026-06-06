using Dapper;
using Mediator;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Models;
using Gamma.Kernel.Extensions;
using System.Data;

namespace Gamma.Kernel.Queries;

public abstract class GetByIdQueryHandlerBase<TQuery, TDto>(
    IDbConnectionFactory connectionFactory
) : IQueryHandler<TQuery, Result<TDto>>
    where TQuery : IGetByIdQuery<TDto>
{
    protected IDbConnectionFactory ConnectionFactory { get; } = connectionFactory;

    protected abstract SqlBuilder Sql { get; }

    public async ValueTask<Result<TDto>> Handle(
        TQuery query,
        CancellationToken ct)
    {
        var sql = Sql;
        sql.Parameters = new { query.Id };

        using var connection = ConnectionFactory.CreateConnection();
        var dto = await connection.QueryFirstOrDefaultAsync<TDto>(sql, cancellationToken: ct);

        if (dto is null)
            return Result<TDto>.Fail($"Entity not found");

        dto = await OnAfterLoad(query, dto, connection, ct);

        return Result<TDto>.Ok(dto);
    }

    protected virtual ValueTask<TDto> OnAfterLoad(
        TQuery query,
        TDto dto,
        IDbConnection connection,
        CancellationToken ct)
        => ValueTask.FromResult(dto);
}