using Gamma.Next.Application.DTOs;
using Gamma.Next.Application.Queries.Customer;

namespace Gamma.Next.Application.Interfaces;

public interface ICustomerQueryService
{
    Task<List<CustomerDto>> GetAsync(CustomerQuery query);
}
