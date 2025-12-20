using System.Data;

namespace Gamma.Kernel.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}



