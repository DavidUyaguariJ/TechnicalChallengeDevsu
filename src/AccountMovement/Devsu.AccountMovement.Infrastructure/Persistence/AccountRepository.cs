using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devsu.AccountMovement.Infrastructure.Persistence;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsDbContext _dbContext;

    public AccountRepository(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Accounts.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Account?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _dbContext.Accounts.Update(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
