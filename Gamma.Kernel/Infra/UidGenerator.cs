
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Infra;

public sealed class UidGenerator : IUidGenerator
{
    public Guid New() => Guid.CreateVersion7();
    // public Guid New()
    // {
    //     var randomGuid = Guid.NewGuid();
    //     var bytes = randomGuid.ToByteArray();
    //     var timestamp = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

    //     // Use timestamp in first 8 bytes
    //     Array.Copy(timestamp, 0, bytes, 0, 8);

    //     return new Guid(bytes);
    // }
}
