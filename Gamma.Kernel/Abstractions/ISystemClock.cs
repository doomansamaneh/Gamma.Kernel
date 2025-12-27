namespace Gamma.Kernel.Abstractions;

public interface ISystemClock
{
    DateTime Now { get; }
}
