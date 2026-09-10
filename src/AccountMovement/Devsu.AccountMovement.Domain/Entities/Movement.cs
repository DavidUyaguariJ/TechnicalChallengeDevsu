using Devsu.AccountMovement.Domain.Exceptions;

namespace Devsu.AccountMovement.Domain.Entities;

public sealed class Movement
{
    public long Id { get; private set; }
    public DateTime MovementDate { get; private set; }
    public string MovementType { get; private set; } = default!;
    public decimal Value { get; private set; }
    public decimal Balance { get; private set; }
    public long AccountId { get; private set; }

    private Movement()
    {
    }

    public Movement(DateTime movementDate, string movementType, decimal value, decimal balance, long accountId)
    {
        if (accountId <= 0)
        {
            throw new DomainException("Account id is required.");
        }

        MovementDate = movementDate;
        AccountId = accountId;
        Balance = balance;

        SetTypeAndValue(movementType, value);
    }

    public void Update(string movementType, decimal value, decimal balance)
    {
        SetTypeAndValue(movementType, value);
        Balance = balance;
    }

    private void SetTypeAndValue(string movementType, decimal value)
    {
        if (string.IsNullOrWhiteSpace(movementType))
        {
            throw new DomainException("Movement type is required.");
        }

        if (value == 0)
        {
            throw new DomainException("Movement value cannot be zero.");
        }

        MovementType = movementType;
        Value = value;
    }
}
