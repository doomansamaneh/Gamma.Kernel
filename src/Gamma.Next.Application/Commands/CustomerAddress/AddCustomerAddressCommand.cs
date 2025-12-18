namespace Gamma.Next.Application.Commands.CustomerAddress;

public class AddCustomerAddressCommand
{
    public Guid CustomerId { get; set; }   // Parent
    public CustomerAddressInput Address { get; set; } = new();
}
