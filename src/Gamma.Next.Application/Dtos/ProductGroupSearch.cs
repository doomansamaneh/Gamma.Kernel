using Gamma.Kernel.Abstractions;

namespace Gamma.Next.Application.DTOs;

public sealed record ProductGroupSearch(
    string? Code,
    string? Title,
    bool? IsActive
) : ISearchModel;
