using Gamma.Next.Domain.Entities;

namespace Gamma.Next.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task CreateAsync(Customer customer);
}
