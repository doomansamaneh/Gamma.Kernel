using Gamma.Next.Application.DTOs;
using Gamma.Next.Application.Interfaces;
using Gamma.Next.Domain.Interfaces;

namespace Gamma.Next.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly ICustomerAddressRepository _addressRepo;

    public CustomerService(
        ICustomerRepository customerRepo,
        ICustomerAddressRepository addressRepo)
    {
        _customerRepo = customerRepo;
        _addressRepo = addressRepo;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null)
            return null;

        var addresses = await _addressRepo.GetByCustomerIdAsync(id);

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Addresses = addresses.Select(a => new CustomerAddressDto
            {
                Id = a.Id,
                City = a.City,
                AddressLine = a.AddressLine
            }).ToList()
        };
    }
}
