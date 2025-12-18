using System;

namespace Gamma.Kernel.Models;

public class LogActorModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Ip { get; set; }
    public string UserAgent { get; set; }
    public string DeviceName { get; set; }
    public string RoleTitle { get; set; }
}