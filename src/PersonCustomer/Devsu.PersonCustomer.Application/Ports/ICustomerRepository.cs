using Devsu.PersonCustomer.Domain.Entities;

namespace Devsu.PersonCustomer.Application.Ports;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdentificationAsync(string identification, CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);

    Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default);
}
