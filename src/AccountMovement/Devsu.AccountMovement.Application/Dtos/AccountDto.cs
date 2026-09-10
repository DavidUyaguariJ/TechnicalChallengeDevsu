namespace Devsu.AccountMovement.Application.Dtos;

public record AccountDto(
    long Id,
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    bool Status,
    long CustomerId);
