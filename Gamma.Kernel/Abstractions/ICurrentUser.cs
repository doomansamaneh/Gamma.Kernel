using Gamma.Kernel.Models;

namespace Gamma.Kernel.Abstractions;

public interface ICurrentUser
{
    LogActorModel GetActor();
    string GetUserName();
    Guid GetUserId();
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Permissions { get; }
}
