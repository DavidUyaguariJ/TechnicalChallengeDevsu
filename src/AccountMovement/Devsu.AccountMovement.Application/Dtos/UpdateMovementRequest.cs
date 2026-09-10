namespace Devsu.AccountMovement.Application.Dtos;

public record UpdateMovementRequest(string MovementType, decimal Value, decimal Balance);
