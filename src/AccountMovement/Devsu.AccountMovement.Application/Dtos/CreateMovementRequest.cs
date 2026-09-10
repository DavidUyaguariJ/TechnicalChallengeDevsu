namespace Devsu.AccountMovement.Application.Dtos;

public record CreateMovementRequest(
    DateTime MovementDate,
    string MovementType,
    decimal Value,
    long AccountId);
