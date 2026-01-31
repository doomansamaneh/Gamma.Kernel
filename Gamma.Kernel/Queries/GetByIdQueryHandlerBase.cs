using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Mediator;

namespace Gamma.Kernel.Queries;

public abstract class GetByIdQueryHandlerBase<TQuery, TDto>(
    IDbConnectionFactory connectionFactory
) : IQueryHandler<TQuery, TDto>
    where TQuery : IGetByIdQuery<TDto>
{
    protected IDbConnectionFactory ConnectionFactory { get; } = connectionFactory;

    protected abstract SqlBuilder Sql { get; }

    public async ValueTask<TDto> Handle(TQuery query, CancellationToken ct)
    {
        using var connection = ConnectionFactory.CreateConnection();

        var dto = await connection.QueryFirstAsync<TDto>(
            new CommandDefinition(
                Sql.ToString(),
                new { query.Id },
                cancellationToken: ct
            )
        );

        return await OnAfterLoad(query, dto, ct);
    }

    protected virtual ValueTask<TDto> OnAfterLoad(
        TQuery query,
        TDto dto,
        CancellationToken ct) => ValueTask.FromResult(dto);
}
