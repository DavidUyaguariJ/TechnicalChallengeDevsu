using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.AccountMovement.Api.Controllers;

[ApiController]
[Route("cuentas")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountDto>>> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetAllAsync(cancellationToken);
        return Ok(accounts);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AccountDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var account = await _accountService.GetByIdAsync(id, cancellationToken);
        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var created = await _accountService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AccountDto>> Update(long id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var updated = await _accountService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }
}
