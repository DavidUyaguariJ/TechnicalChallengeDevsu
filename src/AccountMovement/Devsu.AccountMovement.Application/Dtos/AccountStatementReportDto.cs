namespace Devsu.AccountMovement.Application.Dtos;

public record AccountStatementReportDto(
    long CustomerId,
    string CustomerName,
    DateTime StartDate,
    DateTime EndDate,
    IReadOnlyList<AccountStatementDto> Accounts);
