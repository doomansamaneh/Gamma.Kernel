using Gamma.Kernel.Abstractions;

namespace Gamma.Next.Infra.Data;

internal sealed class DapperUnitOfWorkFactory(IDbConnectionFactory connectionFactory) : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public IUnitOfWork Create()
    {
        return new DapperUnitOfWork(_connectionFactory);
    }
}

