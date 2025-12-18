using System.Data;
using Dapper;
using Gamma.Next.Application.Commands.Customer;
using Gamma.Next.Application.Commands.CustomerAddress;
using Gamma.Next.Application.Commands.Shared;
using Gamma.Next.Application.Interfaces;
using Gamma.Next.Domain.Entities;

namespace Gamma.Next.Infra.Services;

public class CustomerCommandService(IDbConnection dbConnection) : ICustomerCommandService
{
    private readonly IDbConnection _dbConnection = dbConnection;

    // ----------------------------
    // Customer operations
    // ----------------------------
    public async Task<Guid> AddCustomerAsync(AddCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var customerId = Guid.NewGuid();

        var sqlCustomer = @"INSERT INTO Customers (Id, Code, Name, Alias, Type) 
                            VALUES (@Id, @Code, @Name, @Alias, @Type)";
        await _dbConnection.ExecuteAsync(sqlCustomer, new
        {
            Id = customerId,
            command.Customer.Code,
            command.Customer.Name,
            command.Customer.Alias,
            Type = (int)command.Customer.Type
        });

        // Insert addresses if any
        foreach (var addr in command.Customer.Addresses)
        {
            await AddAddressInternalAsync(customerId, addr);
        }

        return customerId;
    }

    public async Task<bool> EditCustomerAsync(EditCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var sqlUpdate = @"UPDATE Customers 
                          SET Code = @Code, Name = @Name, Alias = @Alias, Type = @Type 
                          WHERE Id = @Id";

        await _dbConnection.ExecuteAsync(sqlUpdate, new
        {
            command.Id,
            command.Customer.Code,
            command.Customer.Name,
            command.Customer.Alias,
            Type = (int)command.Customer.Type
        });

        // foreach (var addr in command.Customer.Addresses)
        // {
        //     if (addr.Id == Guid.Empty) // new address
        //         await AddAddressInternalAsync(command.Id, addr);
        //     else // existing address
        //         await EditAddressInternalAsync(addr.Id, addr);
        // }
        return true;
    }

    public async Task<bool> DeleteCustomerAsync(DeleteCommand command, CancellationToken cancellationToken = default)
    {
        // Delete addresses first (FK constraint)
        var sqlDeleteAddresses = "DELETE FROM CustomerAddresses WHERE CustomerId = @CustomerId";
        await _dbConnection.ExecuteAsync(sqlDeleteAddresses, new { CustomerId = command.Id });

        var sqlDeleteCustomer = "DELETE FROM Customers WHERE Id = @Id";
        await _dbConnection.ExecuteAsync(sqlDeleteCustomer, new { command.Id });
        return true;
    }

    // ----------------------------
    // CustomerAddress operations
    // ----------------------------
    public async Task<Guid> AddAddressAsync(AddCustomerAddressCommand command, CancellationToken cancellationToken = default)
    {
        return await AddAddressInternalAsync(command.CustomerId, command.Address);

    }

    public async Task<bool> EditAddressAsync(EditCustomerAddressCommand command, CancellationToken cancellationToken = default)
    {
        await EditAddressInternalAsync(command.Id, command.Address);
        return true;
    }

    public async Task<bool> DeleteAddressAsync(DeleteCommand command, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM CustomerAddresses WHERE Id = @Id";
        await _dbConnection.ExecuteAsync(sql, new { command.Id });
        return true;
    }

    // ----------------------------
    // Internal helpers
    // ----------------------------
    private async Task<Guid> AddAddressInternalAsync(Guid customerId, CustomerAddressInput addr)
    {
        var addressId = Guid.NewGuid();
        var sql = @"INSERT INTO CustomerAddresses (Id, CustomerId, City, AddressLine)
                    VALUES (@Id, @CustomerId, @City, @AddressLine)";
        await _dbConnection.ExecuteAsync(sql, new
        {
            Id = addressId,
            CustomerId = customerId,
            addr.City,
            addr.AddressLine
        });

        return addressId;
    }

    private async Task EditAddressInternalAsync(Guid addressId, CustomerAddressInput addr)
    {
        var sql = @"UPDATE CustomerAddresses 
                    SET City = @City, AddressLine = @AddressLine 
                    WHERE Id = @Id";
        await _dbConnection.ExecuteAsync(sql, new
        {
            Id = addressId,
            addr.City,
            addr.AddressLine
        });
    }
}
