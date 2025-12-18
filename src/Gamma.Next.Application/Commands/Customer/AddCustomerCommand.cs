namespace Gamma.Next.Application.Commands.Customer;

public class AddCustomerCommand
{
    public CustomerInput Customer { get; set; } = new();
}
