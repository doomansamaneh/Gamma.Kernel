namespace Gamma.Next.Application.Commands.Customer;

public class EditCustomerCommand
{
    public Guid Id { get; set; }
    public CustomerInput Customer { get; set; } = new();
}