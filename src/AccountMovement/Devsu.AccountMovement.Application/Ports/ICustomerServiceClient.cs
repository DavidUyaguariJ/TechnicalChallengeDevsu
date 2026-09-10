using Devsu.Shared.Contracts;

namespace Devsu.AccountMovement.Application.Ports;

public interface ICustomerServiceClient
{
    Task<CustomerSummary?> GetByIdAsync(long customerId, CancellationToken cancellationToken = default);
}
