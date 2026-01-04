using Gamma.Kernel.Abstractions;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Models;
using Mediator;

namespace Gamma.Next.Application.ProductGroup.Commands;

public class DeleteProductGroupCommand
    : IAuditableCommand,
        ICommand<Result<int>>
{
    public Guid Id { get; set; }
}