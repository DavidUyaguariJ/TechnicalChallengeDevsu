using Devsu.AccountMovement.Domain.Entities;
using Devsu.AccountMovement.Domain.Exceptions;
using FluentAssertions;

namespace Devsu.AccountMovement.UnitTests.Domain;

public class AccountTests
{
    private static Account CreateValidAccount() =>
        new(accountNumber: "478758", accountType: "Savings", initialBalance: 2000m, customerId: 1);

    [Fact]
    public void Constructor_WithValidData_CreatesActiveAccount()
    {
        var account = CreateValidAccount();

        account.AccountNumber.Should().Be("478758");
        account.AccountType.Should().Be("Savings");
        account.InitialBalance.Should().Be(2000m);
        account.CustomerId.Should().Be(1);
        account.Status.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithoutAccountNumber_ThrowsDomainException(string? accountNumber)
    {
        var act = () => new Account(accountNumber!, "Savings", 2000m, 1);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithoutAccountType_ThrowsDomainException(string? accountType)
    {
        var act = () => new Account("478758", accountType!, 2000m, 1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithNegativeInitialBalance_ThrowsDomainException()
    {
        var act = () => new Account("478758", "Savings", -1m, 1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithInvalidCustomerId_ThrowsDomainException()
    {
        var act = () => new Account("478758", "Savings", 2000m, 0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesAccountTypeAndStatus()
    {
        var account = CreateValidAccount();

        account.UpdateDetails("Checking", false);

        account.AccountType.Should().Be("Checking");
        account.Status.Should().BeFalse();
    }

    [Fact]
    public void UpdateDetails_WithEmptyAccountType_ThrowsDomainException()
    {
        var account = CreateValidAccount();

        var act = () => account.UpdateDetails(" ", true);

        act.Should().Throw<DomainException>();
    }
}
