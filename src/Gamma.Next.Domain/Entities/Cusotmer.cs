using Gamma.Next.Domain.Enums;
using Gamma.Next.Domain.Attributes;
using Gamma.Next.Domain.Common;

namespace Gamma.Next.Domain.Entities;

[Schema("Crm")]
public class Customer : ConcurrencyEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Alias { get; set; }
    public int TypeId { get; set; }
    public bool IsActive { get; set; }
}
