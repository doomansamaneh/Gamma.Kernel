using System.Data;
using Gamma.Kernel.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Gamma.Next.Infra.Data;

internal sealed class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("Default")!;

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
