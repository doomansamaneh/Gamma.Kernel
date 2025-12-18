namespace Gamma.Next.Application.Commands.CustomerAddress;

public class EditCustomerAddressCommand
{
    public Guid Id { get; set; }
    public CustomerAddressInput Address { get; set; } = new();
}
