using Gamma.Next.Application.Commands.Customer;
using Gamma.Next.Application.Commands.CustomerAddress;
using Gamma.Next.Application.Commands.Shared;

namespace Gamma.Next.Application.Interfaces;

public interface ICustomerCommandService
{
    Task<Guid> AddCustomerAsync(AddCustomerCommand command, CancellationToken cancellationToken = default);
    Task<bool> EditCustomerAsync(EditCustomerCommand command, CancellationToken cancellationToken = default);
    Task<bool> DeleteCustomerAsync(DeleteCommand command, CancellationToken cancellationToken = default);

    // ----------------------------
    // CustomerAddress operations
    // ----------------------------
    Task<Guid> AddAddressAsync(AddCustomerAddressCommand command, CancellationToken cancellationToken = default);
    Task<bool> EditAddressAsync(EditCustomerAddressCommand command, CancellationToken cancellationToken = default);
    Task<bool> DeleteAddressAsync(DeleteCommand command, CancellationToken cancellationToken = default);
}
