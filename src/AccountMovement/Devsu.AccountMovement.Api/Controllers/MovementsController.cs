using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.AccountMovement.Api.Controllers;

[ApiController]
[Route("movimientos")]
public class MovementsController : ControllerBase
{
    private readonly IMovementService _movementService;

    public MovementsController(IMovementService movementService)
    {
        _movementService = movementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MovementDto>>> GetAll(CancellationToken cancellationToken)
    {
        var movements = await _movementService.GetAllAsync(cancellationToken);
        return Ok(movements);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MovementDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var movement = await _movementService.GetByIdAsync(id, cancellationToken);
        return Ok(movement);
    }

    [HttpPost]
    public async Task<ActionResult<MovementDto>> Create(CreateMovementRequest request, CancellationToken cancellationToken)
    {
        var created = await _movementService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<MovementDto>> Update(long id, UpdateMovementRequest request, CancellationToken cancellationToken)
    {
        var updated = await _movementService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }
}
