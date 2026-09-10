using Devsu.AccountMovement.Application.Exceptions;
using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Application.Services;
using Devsu.AccountMovement.Domain.Entities;
using Devsu.AccountMovement.Domain.Exceptions;
using Devsu.Shared.Contracts;
using FluentAssertions;
using Moq;

namespace Devsu.AccountMovement.UnitTests.Application;

public class ReportServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly Mock<IMovementRepository> _movementRepository = new();
    private readonly Mock<ICustomerServiceClient> _customerServiceClient = new();
    private readonly ReportService _sut;

    public ReportServiceTests()
    {
        _sut = new ReportService(_accountRepository.Object, _movementRepository.Object, _customerServiceClient.Object);
        _customerServiceClient.Setup(c => c.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomerSummary(1, "Jose Lema", true));
    }

    private static Account CreateAccount(long id = 1, string accountNumber = "478758", decimal initialBalance = 2000m)
    {
        var account = new Account(accountNumber, "Savings", initialBalance, 1);
        typeof(Account).GetProperty(nameof(Account.Id))!.SetValue(account, id);
        return account;
    }

    private static Movement CreateMovement(long accountId, DateTime date, decimal value, decimal balance) =>
        new(date, value >= 0 ? "DEPOSIT" : "WITHDRAWAL", value, balance, accountId);

    [Fact]
    public async Task GetAccountStatementAsync_WhenCustomerHasAccountsAndMovements_ReturnsReportWithCurrentBalanceFromLastMovement()
    {
        var account = CreateAccount();
        _accountRepository.Setup(r => r.GetByCustomerIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account> { account });

        var movements = new List<Movement>
        {
            CreateMovement(account.Id, new DateTime(2026, 1, 5), -575m, 1425m),
            CreateMovement(account.Id, new DateTime(2026, 1, 10), 600m, 2025m),
        };
        _movementRepository.Setup(r => r.GetByAccountIdsAndDateRangeAsync(
                It.Is<IReadOnlyList<long>>(ids => ids.Single() == account.Id),
                new DateTime(2026, 1, 1),
                new DateTime(2026, 1, 31),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(movements);

        var result = await _sut.GetAccountStatementAsync(1, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        result.CustomerId.Should().Be(1);
        result.CustomerName.Should().Be("Jose Lema");
        result.Accounts.Should().HaveCount(1);
        result.Accounts[0].Movements.Should().HaveCount(2);
        result.Accounts[0].CurrentBalance.Should().Be(2025m);
    }

    [Fact]
    public async Task GetAccountStatementAsync_WhenAccountHasNoMovementsInRange_ReturnsCurrentBalanceFromInitialBalance()
    {
        var account = CreateAccount(initialBalance: 100m);
        _accountRepository.Setup(r => r.GetByCustomerIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account> { account });
        _movementRepository.Setup(r => r.GetByAccountIdsAndDateRangeAsync(
                It.IsAny<IReadOnlyList<long>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movement>());

        var result = await _sut.GetAccountStatementAsync(1, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        result.Accounts[0].CurrentBalance.Should().Be(100m);
        result.Accounts[0].Movements.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAccountStatementAsync_WhenCustomerHasNoAccounts_ReturnsReportWithEmptyAccounts()
    {
        _accountRepository.Setup(r => r.GetByCustomerIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>());

        var result = await _sut.GetAccountStatementAsync(1, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        result.Accounts.Should().BeEmpty();
        _movementRepository.Verify(
            r => r.GetByAccountIdsAndDateRangeAsync(It.IsAny<IReadOnlyList<long>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAccountStatementAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        _customerServiceClient.Setup(c => c.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerSummary?)null);

        var act = () => _sut.GetAccountStatementAsync(99, new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        await act.Should().ThrowAsync<NotFoundException>();
        _accountRepository.Verify(r => r.GetByCustomerIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAccountStatementAsync_WhenStartDateIsAfterEndDate_ThrowsDomainException()
    {
        var act = () => _sut.GetAccountStatementAsync(1, new DateTime(2026, 1, 31), new DateTime(2026, 1, 1));

        await act.Should().ThrowAsync<DomainException>();
        _customerServiceClient.Verify(c => c.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
