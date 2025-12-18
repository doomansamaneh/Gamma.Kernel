using Gamma.Next.Domain.Enums;
using Gamma.Next.Domain.Attributes;
using Gamma.Next.Domain.Common;

namespace Gamma.Next.Domain.Entities;

[Schema("Crm")]
public class CustomerAddress : ConcurrencyEntity
{
    public Guid CustomerId { get; set; }
    public string City { get; set; } = default!;
    public string AddressLine { get; set; } = default!;
    public bool IsPrimary { get; set; }
}
