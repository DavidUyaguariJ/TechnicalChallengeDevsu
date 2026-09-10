using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devsu.AccountMovement.Infrastructure.Persistence;

public class MovementRepository : IMovementRepository
{
    private readonly AccountsDbContext _dbContext;

    public MovementRepository(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Movement>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Movements.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Movement?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _dbContext.Movements.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<Movement?> GetLastByAccountIdAsync(long accountId, CancellationToken cancellationToken = default) =>
        await _dbContext.Movements
            .AsNoTracking()
            .Where(m => m.AccountId == accountId)
            .OrderByDescending(m => m.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(Movement movement, CancellationToken cancellationToken = default)
    {
        await _dbContext.Movements.AddAsync(movement, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Movement movement, CancellationToken cancellationToken = default)
    {
        _dbContext.Movements.Update(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
