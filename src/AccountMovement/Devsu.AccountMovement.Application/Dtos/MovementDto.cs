namespace Devsu.AccountMovement.Application.Dtos;

public record MovementDto(
    long Id,
    DateTime MovementDate,
    string MovementType,
    decimal Value,
    decimal Balance,
    long AccountId);
