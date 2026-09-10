namespace Devsu.AccountMovement.Application.Dtos;

public record CreateAccountRequest(
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    long CustomerId);
