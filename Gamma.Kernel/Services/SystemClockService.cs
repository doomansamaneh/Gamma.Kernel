
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Services;

public class SystemClockService : ISystemClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
