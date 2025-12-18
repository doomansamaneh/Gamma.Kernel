namespace Gamma.Kernel.Abstractions;

public interface ISystemClock
{
    DateTime UtcNow { get; }
}
