using System.Data;
using Gamma.Kernel.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Gamma.Next.Infra.Data;

internal sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")!;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
