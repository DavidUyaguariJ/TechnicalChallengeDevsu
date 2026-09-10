using Devsu.AccountMovement.Domain.Exceptions;

namespace Devsu.AccountMovement.Domain.Entities;

public sealed class Account
{
    public long Id { get; private set; }
    public string AccountNumber { get; private set; } = default!;
    public string AccountType { get; private set; } = default!;
    public decimal InitialBalance { get; private set; }
    public bool Status { get; private set; }
    public long CustomerId { get; private set; }

    private Account()
    {
    }

    public Account(string accountNumber, string accountType, decimal initialBalance, long customerId)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new DomainException("Account number is required.");
        }

        if (initialBalance < 0)
        {
            throw new DomainException("Initial balance cannot be negative.");
        }

        if (customerId <= 0)
        {
            throw new DomainException("Customer id is required.");
        }

        AccountNumber = accountNumber;
        InitialBalance = initialBalance;
        CustomerId = customerId;
        Status = true;

        SetAccountType(accountType);
    }

    public void UpdateDetails(string accountType, bool status)
    {
        SetAccountType(accountType);
        Status = status;
    }

    private void SetAccountType(string accountType)
    {
        if (string.IsNullOrWhiteSpace(accountType))
        {
            throw new DomainException("Account type is required.");
        }

        AccountType = accountType;
    }
}
