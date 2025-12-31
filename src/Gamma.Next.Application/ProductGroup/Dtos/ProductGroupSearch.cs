using Gamma.Kernel.Abstractions;

namespace Gamma.Next.Application.ProductGroup.Dtos;

public sealed record ProductGroupSearch(
    string? Code,
    string? Title,
    bool? IsActive
) : ISearchModel;
