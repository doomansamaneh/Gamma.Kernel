using Dapper;
using Gamma.Next.Domain.Entities;
using Gamma.Next.Domain.Interfaces;

namespace Gamma.Next.Infra.Data;

public class CustomerAddressRepository : ICustomerAddressRepository
{
    private readonly DapperContext _context;

    public CustomerAddressRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerAddress>> GetByCustomerIdAsync(Guid customerId)
    {
        const string sql =
            @"SELECT Id, CustomerId, City, AddressLine
              FROM CustomerAddresses
              WHERE CustomerId = @CustomerId";

        using var conn = _context.CreateConnection();
        return (await conn.QueryAsync<CustomerAddress>(
            sql, new { CustomerId = customerId })).ToList();
    }

    public async Task CreateAsync(CustomerAddress address)
    {
        const string sql =
            @"INSERT INTO CustomerAddresses
              (Id, CustomerId, City, AddressLine)
              VALUES (@Id, @CustomerId, @City, @AddressLine)";

        using var conn = _context.CreateConnection();
        await conn.ExecuteAsync(sql, address);
    }
}
