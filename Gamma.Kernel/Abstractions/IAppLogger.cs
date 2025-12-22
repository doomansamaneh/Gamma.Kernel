namespace Gamma.Kernel.Abstractions;

public interface IAppLogger
{
    void Info(string message, object? data = null);
    void Warning(string message, object? data = null);
    void Error(string message, Exception? exception = null, object? data = null);
}
