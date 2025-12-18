using System.Data;
using Microsoft.Data.SqlClient;

namespace Gamma.Next.Infra.Data;

public class DapperContext(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
