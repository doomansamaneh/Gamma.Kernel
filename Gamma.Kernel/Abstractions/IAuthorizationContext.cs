namespace Gamma.Kernel.Abstractions;

public interface IAuthorizationContext
{
    bool IsAuthorized { get; }
    void MarkAuthorized();
}