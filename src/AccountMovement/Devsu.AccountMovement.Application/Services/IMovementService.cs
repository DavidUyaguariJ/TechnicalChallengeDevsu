using Devsu.AccountMovement.Application.Dtos;

namespace Devsu.AccountMovement.Application.Services;

public interface IMovementService
{
    Task<IReadOnlyList<MovementDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<MovementDto> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<MovementDto> CreateAsync(CreateMovementRequest request, CancellationToken cancellationToken = default);

    Task<MovementDto> UpdateAsync(long id, UpdateMovementRequest request, CancellationToken cancellationToken = default);
}
