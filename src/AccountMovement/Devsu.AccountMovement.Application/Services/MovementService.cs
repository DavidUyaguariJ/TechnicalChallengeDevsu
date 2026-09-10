using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Exceptions;
using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Domain.Entities;

namespace Devsu.AccountMovement.Application.Services;

public class MovementService : IMovementService
{
    private readonly IMovementRepository _movementRepository;
    private readonly IAccountRepository _accountRepository;

    public MovementService(IMovementRepository movementRepository, IAccountRepository accountRepository)
    {
        _movementRepository = movementRepository;
        _accountRepository = accountRepository;
    }

    public async Task<IReadOnlyList<MovementDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var movements = await _movementRepository.GetAllAsync(cancellationToken);
        return movements.Select(ToDto).ToList();
    }

    public async Task<MovementDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var movement = await GetExistingMovementAsync(id, cancellationToken);
        return ToDto(movement);
    }

    public async Task<MovementDto> CreateAsync(CreateMovementRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken)
            ?? throw new NotFoundException($"Account '{request.AccountId}' was not found.");

        var lastMovement = await _movementRepository.GetLastByAccountIdAsync(request.AccountId, cancellationToken);
        var currentBalance = lastMovement?.Balance ?? account.InitialBalance;
        var newBalance = currentBalance + request.Value;

        if (newBalance < 0)
        {
            throw new InsufficientBalanceException("Saldo no disponible");
        }

        var movement = new Movement(request.MovementDate, request.MovementType, request.Value, newBalance, request.AccountId);

        await _movementRepository.AddAsync(movement, cancellationToken);

        return ToDto(movement);
    }

    public async Task<MovementDto> UpdateAsync(long id, UpdateMovementRequest request, CancellationToken cancellationToken = default)
    {
        var movement = await GetExistingMovementAsync(id, cancellationToken);

        movement.Update(request.MovementType, request.Value, request.Balance);

        await _movementRepository.UpdateAsync(movement, cancellationToken);

        return ToDto(movement);
    }

    private async Task<Movement> GetExistingMovementAsync(long id, CancellationToken cancellationToken)
    {
        return await _movementRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Movement '{id}' was not found.");
    }

    private static MovementDto ToDto(Movement movement) => new(
        movement.Id,
        movement.MovementDate,
        movement.MovementType,
        movement.Value,
        movement.Balance,
        movement.AccountId);
}
