using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Exceptions;
using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Domain.Entities;
using Devsu.AccountMovement.Domain.Exceptions;

namespace Devsu.AccountMovement.Application.Services;

public class ReportService : IReportService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMovementRepository _movementRepository;
    private readonly ICustomerServiceClient _customerServiceClient;

    public ReportService(
        IAccountRepository accountRepository,
        IMovementRepository movementRepository,
        ICustomerServiceClient customerServiceClient)
    {
        _accountRepository = accountRepository;
        _movementRepository = movementRepository;
        _customerServiceClient = customerServiceClient;
    }

    public async Task<AccountStatementReportDto> GetAccountStatementAsync(
        long customerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        if (startDate > endDate)
        {
            throw new DomainException("Start date must not be later than end date.");
        }

        var customer = await _customerServiceClient.GetByIdAsync(customerId, cancellationToken)
            ?? throw new NotFoundException($"Customer '{customerId}' was not found.");

        var accounts = await _accountRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        var accountIds = accounts.Select(a => a.Id).ToList();
        var movements = accountIds.Count > 0
            ? await _movementRepository.GetByAccountIdsAndDateRangeAsync(accountIds, startDate, endDate, cancellationToken)
            : [];
        var movementsByAccountId = movements.ToLookup(m => m.AccountId);

        var accountStatements = accounts
            .Select(account => ToAccountStatementDto(account, movementsByAccountId[account.Id].ToList()))
            .ToList();

        return new AccountStatementReportDto(customerId, customer.Name, startDate, endDate, accountStatements);
    }

    private static AccountStatementDto ToAccountStatementDto(Account account, IReadOnlyList<Movement> movements)
    {
        var currentBalance = movements.Count > 0 ? movements[^1].Balance : account.InitialBalance;

        return new AccountStatementDto(
            account.Id,
            account.AccountNumber,
            account.AccountType,
            account.InitialBalance,
            account.Status,
            currentBalance,
            movements.Select(m => new MovementStatementDto(m.Id, m.MovementDate, m.MovementType, m.Value, m.Balance)).ToList());
    }
}
