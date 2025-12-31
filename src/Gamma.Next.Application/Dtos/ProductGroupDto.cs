namespace Gamma.Next.Application.DTOs;

public sealed record ProductGroupDto(
    Guid Id,
    string Code,
    string Title,
    bool IsActive
);

