using Devsu.AccountMovement.Application.Dtos;

namespace Devsu.AccountMovement.Application.Services;

public interface IReportService
{
    Task<AccountStatementReportDto> GetAccountStatementAsync(
        long customerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
