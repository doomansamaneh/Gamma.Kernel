using Gamma.Next.Domain.Entities;

namespace Gamma.Next.Domain.Interfaces;

public interface ICustomerAddressRepository
{
    Task<List<CustomerAddress>> GetByCustomerIdAsync(Guid customerId);
    Task CreateAsync(CustomerAddress address);
}