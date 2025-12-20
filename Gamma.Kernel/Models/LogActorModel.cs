using System;

namespace Gamma.Kernel.Models;

public class LogActorModel
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Ip { get; init; }
    public string? UserAgent { get; init; }
    public string? DeviceName { get; init; }
}