namespace Gamma.Next.Application.DTOs;

public class CustomerAddressDto
{
    public Guid Id { get; set; }
    public string City { get; set; } = default!;
    public string AddressLine { get; set; } = default!;
}