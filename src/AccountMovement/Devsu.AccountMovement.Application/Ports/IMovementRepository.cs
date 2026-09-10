using Devsu.AccountMovement.Domain.Entities;

namespace Devsu.AccountMovement.Application.Ports;

public interface IMovementRepository
{
    Task<IReadOnlyList<Movement>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Movement?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<Movement?> GetLastByAccountIdAsync(long accountId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Movement>> GetByAccountIdsAndDateRangeAsync(
        IReadOnlyList<long> accountIds,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(Movement movement, CancellationToken cancellationToken = default);

    Task UpdateAsync(Movement movement, CancellationToken cancellationToken = default);
}
