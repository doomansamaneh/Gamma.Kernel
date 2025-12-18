namespace Gamma.Next.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Alias { get; set; }
    public List<CustomerAddressDto> Addresses { get; set; } = new();
}
