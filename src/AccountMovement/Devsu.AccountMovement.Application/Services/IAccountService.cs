using Devsu.AccountMovement.Application.Dtos;

namespace Devsu.AccountMovement.Application.Services;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AccountDto> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<AccountDto> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);

    Task<AccountDto> UpdateAsync(long id, UpdateAccountRequest request, CancellationToken cancellationToken = default);
}
