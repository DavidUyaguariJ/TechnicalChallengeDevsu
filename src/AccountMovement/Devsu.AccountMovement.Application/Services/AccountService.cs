using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Exceptions;
using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Domain.Entities;

namespace Devsu.AccountMovement.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        return accounts.Select(ToDto).ToList();
    }

    public async Task<AccountDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var account = await GetExistingAccountAsync(id, cancellationToken);
        return ToDto(account);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var existingAccount = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber, cancellationToken);
        if (existingAccount is not null)
        {
            throw new ConflictException($"An account with number '{request.AccountNumber}' already exists.");
        }

        var account = new Account(request.AccountNumber, request.AccountType, request.InitialBalance, request.CustomerId);

        await _accountRepository.AddAsync(account, cancellationToken);

        return ToDto(account);
    }

    public async Task<AccountDto> UpdateAsync(long id, UpdateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await GetExistingAccountAsync(id, cancellationToken);

        account.UpdateDetails(request.AccountType, request.Status);

        await _accountRepository.UpdateAsync(account, cancellationToken);

        return ToDto(account);
    }

    private async Task<Account> GetExistingAccountAsync(long id, CancellationToken cancellationToken)
    {
        return await _accountRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Account '{id}' was not found.");
    }

    private static AccountDto ToDto(Account account) => new(
        account.Id,
        account.AccountNumber,
        account.AccountType,
        account.InitialBalance,
        account.Status,
        account.CustomerId);
}
