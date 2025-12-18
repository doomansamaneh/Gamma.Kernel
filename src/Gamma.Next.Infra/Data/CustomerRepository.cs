using Dapper;
using Gamma.Next.Domain.Entities;
using Gamma.Next.Domain.Interfaces;

namespace Gamma.Next.Infra.Data;

public class CustomerRepository : ICustomerRepository
{
    private readonly DapperContext _context;

    public CustomerRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        const string sql =
            "SELECT Id, Name FROM Customers WHERE Id = @Id";

        using var conn = _context.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
    }

    public async Task CreateAsync(Customer customer)
    {
        const string sql =
            "INSERT INTO Customers (Id, Name) VALUES (@Id, @Name)";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, customer);
    }
}
