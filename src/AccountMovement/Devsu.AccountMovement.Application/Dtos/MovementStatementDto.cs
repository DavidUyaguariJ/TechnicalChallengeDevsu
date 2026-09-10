namespace Devsu.AccountMovement.Application.Dtos;

public record MovementStatementDto(
    long Id,
    DateTime MovementDate,
    string MovementType,
    decimal Value,
    decimal Balance);
