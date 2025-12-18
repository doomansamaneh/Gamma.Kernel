using Dapper;
using Gamma.Next.Application.DTOs;
using Gamma.Next.Application.Interfaces;
using Gamma.Next.Application.Queries.Customer;
using Gamma.Next.Infra.Data;

namespace Gamma.Next.Infra.Services;

public class CustomerQueryService(DapperContext context) : ICustomerQueryService
{
    private readonly DapperContext _context = context;

    public async Task<List<CustomerDto>> GetAsync(CustomerQuery query)
    {
        using var conn = _context.CreateConnection();

        var sql = @"
            SELECT c.Id, c.Name, a.Id AS AddressId, a.City, a.AddressLine
            FROM Customers c
            LEFT JOIN CustomerAddresses a ON a.CustomerId=c.Id
            WHERE 1=1";

        if (!string.IsNullOrEmpty(query.Name))
            sql += " AND c.Name LIKE @Name";

        if (!string.IsNullOrEmpty(query.City))
            sql += " AND a.City LIKE @City";

        sql += " ORDER BY c.Name OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var data = await conn.QueryAsync<CustomerDto, CustomerAddressDto, CustomerDto>(
            sql,
            (c, a) =>
            {
                if (c.Addresses == null)
                    c.Addresses = new List<CustomerAddressDto>();
                if (a != null)
                    c.Addresses.Add(a);
                return c;
            },
            new
            {
                Name = $"%{query.Name}%",
                City = $"%{query.City}%",
                Offset = (query.Page - 1) * query.PageSize,
                PageSize = query.PageSize
            },
            splitOn: "AddressId"
        );

        return data.DistinctBy(c => c.Id).ToList();
    }
}
