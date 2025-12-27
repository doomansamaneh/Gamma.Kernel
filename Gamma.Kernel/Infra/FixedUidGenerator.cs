
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Infra;

public sealed class FixedUidGenerator : IUidGenerator
{
    public Guid New() => Guid.Parse("00000000-0000-0000-0000-000000000001");
}
