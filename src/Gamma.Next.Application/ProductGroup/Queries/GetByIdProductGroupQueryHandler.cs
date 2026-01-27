using Dapper;
using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Dapper;
using Gamma.Next.Application.ProductGroup.Dtos;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Queries;

internal sealed class GetByIdProductGroupQueryHandler(IDbConnectionFactory connectionFactory)
        : IQueryHandler<GetByIdProductGroupQuery, ProductGroupDto>
{
    public async ValueTask<ProductGroupDto> Handle(
        GetByIdProductGroupQuery query,
        CancellationToken cancellationToken)
    {
        var sql = SQL.Select("pg.*")
                    .From("ast.ProductGroup pg")
                    .Where($"pg.Id=@id");
        sql.AddParameter("id", query.Id);
        using var _connection = connectionFactory.CreateConnection();

        return await _connection.QueryFirstAsync<ProductGroupDto>(
            new CommandDefinition(sql.ToString(), sql.Parameters, cancellationToken: cancellationToken)
        );
    }
}
