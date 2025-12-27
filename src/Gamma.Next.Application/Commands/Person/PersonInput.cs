namespace Gamma.Next.Application.Commands.Person;

public class PersonInput
{
    public string NationalCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsActive { get; set; }
}