namespace Gamma.Next.Application.ProductGroup.Dtos;

public sealed record ProductGroupDto(
    Guid Id,
    string Code,
    string Title,
    bool IsActive
);

