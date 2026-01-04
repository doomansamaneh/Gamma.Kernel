using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Persistence;
using Mediator;

namespace Gamma.Kernel.Pipelines;

public sealed class UnitOfWorkPipeline<TMessage, TResponse>(IDbConnectionFactory connectionFactory)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IBaseCommand
{

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken ct)
    {
        var isRoot = !UnitOfWorkScope.HasCurrent;

        using var connection = isRoot ? connectionFactory.CreateConnection() : null;
        using var transaction = isRoot ? connection!.BeginTransaction() : null;

        using (UnitOfWorkScope.CreateScope(connection, transaction))
        {
            var uow = UnitOfWorkScope.Current;

            var response = await next(message, ct);

            if (isRoot)
            {
                try
                {
                    transaction!.Commit();
                }
                catch
                {
                    transaction!.Rollback();
                    throw;
                }

                // Run all OnCommitted actions after transaction completes
                await uow.NotifyCommittedAsync(ct);
            }

            return response;
        }
    }
}