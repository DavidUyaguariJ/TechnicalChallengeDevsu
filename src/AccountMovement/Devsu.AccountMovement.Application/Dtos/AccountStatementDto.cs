namespace Devsu.AccountMovement.Application.Dtos;

public record AccountStatementDto(
    long Id,
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    bool Status,
    decimal CurrentBalance,
    IReadOnlyList<MovementStatementDto> Movements);
