using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Persistence;

public sealed class DapperUnitOfWorkFactory(IDbConnectionFactory connectionFactory) : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public IUnitOfWork Create()
    {
        return new DapperUnitOfWork(_connectionFactory);
    }
}

