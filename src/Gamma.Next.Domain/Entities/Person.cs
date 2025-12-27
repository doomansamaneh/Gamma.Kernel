using Gamma.Kernel.Attributes;
using Gamma.Kernel.Entities;

namespace Gamma.Next.Domain.Entities;

[Schema("Crm")]
public class Person : ConcurrencyEntity
{
    public string NationalCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}
