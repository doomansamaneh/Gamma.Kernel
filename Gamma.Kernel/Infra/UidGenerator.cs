
using Gamma.Kernel.Abstractions;

namespace Gamma.Kernel.Infra;

public sealed class UidGenerator : IUidGenerator
{
    public Guid New() => Guid.CreateVersion7();
}

public sealed class SqlServerUidGenerator : IUidGenerator
{
    public Guid New()
    {
        // COMB Guid for SQL Server
        var guidArray = Guid.NewGuid().ToByteArray();

        var now = DateTime.UtcNow;
        var days = BitConverter.GetBytes((now - new DateTime(1900, 1, 1)).Days);
        var msecs = BitConverter.GetBytes((long)now.TimeOfDay.TotalMilliseconds);

        Array.Reverse(days);
        Array.Reverse(msecs);

        Array.Copy(days, days.Length - 2, guidArray, guidArray.Length - 6, 2);
        Array.Copy(msecs, msecs.Length - 4, guidArray, guidArray.Length - 4, 4);

        return new Guid(guidArray);
    }
}
