using Devsu.AccountMovement.Domain.Entities;

namespace Devsu.AccountMovement.Application.Ports;

public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Account?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

    Task AddAsync(Account account, CancellationToken cancellationToken = default);

    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
}
