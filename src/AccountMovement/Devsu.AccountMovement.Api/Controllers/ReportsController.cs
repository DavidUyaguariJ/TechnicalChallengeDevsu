using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.AccountMovement.Api.Controllers;

[ApiController]
[Route("reportes")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<ActionResult<AccountStatementReportDto>> Get(
        [FromQuery(Name = "cliente")] long clienteId,
        [FromQuery(Name = "fechaInicio")] DateTime fechaInicio,
        [FromQuery(Name = "fechaFin")] DateTime fechaFin,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetAccountStatementAsync(clienteId, fechaInicio, fechaFin, cancellationToken);
        return Ok(report);
    }
}
