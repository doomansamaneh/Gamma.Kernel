using Gamma.Next.Application.Commands.CustomerAddress;
using Gamma.Next.Application.DTOs;
using Gamma.Next.Domain.Enums;

namespace Gamma.Next.Application.Commands.Customer;

public class CustomerInput
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Alias { get; set; }
    public CustomerType Type { get; set; }
    public List<CustomerAddressInput> Addresses { get; set; } = new();
}