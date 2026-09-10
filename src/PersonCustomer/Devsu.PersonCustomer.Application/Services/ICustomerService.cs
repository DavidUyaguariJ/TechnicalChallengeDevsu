using Devsu.PersonCustomer.Application.Dtos;

namespace Devsu.PersonCustomer.Application.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CustomerDto> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);

    Task<CustomerDto> UpdateAsync(long id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
