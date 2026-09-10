using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Exceptions;
using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Application.Services;
using Devsu.AccountMovement.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Devsu.AccountMovement.UnitTests.Application;

public class MovementServiceTests
{
    private readonly Mock<IMovementRepository> _movementRepository = new();
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly MovementService _sut;

    public MovementServiceTests()
    {
        _sut = new MovementService(_movementRepository.Object, _accountRepository.Object);
    }

    private static Account CreateAccount() => new("478758", "Savings", 2000m, 1);

    private static Movement CreateMovement() =>
        new(new DateTime(2026, 1, 22), "DEPOSIT", 600m, 700m, 1);

    [Fact]
    public async Task CreateAsync_WhenAccountHasNoPreviousMovements_ComputesBalanceFromInitialBalance()
    {
        _accountRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateAccount());
        _movementRepository.Setup(r => r.GetLastByAccountIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Movement?)null);
        var request = new CreateMovementRequest(new DateTime(2026, 1, 22), "DEPOSIT", 500m, 1);

        var result = await _sut.CreateAsync(request);

        result.MovementType.Should().Be("DEPOSIT");
        result.Value.Should().Be(500m);
        result.Balance.Should().Be(2500m);
        _movementRepository.Verify(r => r.AddAsync(It.IsAny<Movement>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenAccountHasPreviousMovements_ComputesBalanceFromLastMovement()
    {
        _accountRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateAccount());
        _movementRepository.Setup(r => r.GetLastByAccountIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateMovement());
        var request = new CreateMovementRequest(new DateTime(2026, 1, 23), "WITHDRAWAL", -200m, 1);

        var result = await _sut.CreateAsync(request);

        result.Balance.Should().Be(500m);
    }

    [Fact]
    public async Task CreateAsync_WhenAccountDoesNotExist_ThrowsNotFoundException()
    {
        _accountRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Account?)null);
        var request = new CreateMovementRequest(new DateTime(2026, 1, 22), "DEPOSIT", 600m, 1);

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<NotFoundException>();
        _movementRepository.Verify(r => r.AddAsync(It.IsAny<Movement>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMovementExists_ReturnsDto()
    {
        _movementRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateMovement());

        var result = await _sut.GetByIdAsync(1);

        result.MovementType.Should().Be("DEPOSIT");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMovementDoesNotExist_ThrowsNotFoundException()
    {
        _movementRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Movement?)null);

        var act = () => _sut.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllMovementsMapped()
    {
        _movementRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movement> { CreateMovement(), CreateMovement() });

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_WhenMovementExists_UpdatesAndReturnsDto()
    {
        var movement = CreateMovement();
        _movementRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(movement);
        var request = new UpdateMovementRequest("WITHDRAWAL", -100m, 600m);

        var result = await _sut.UpdateAsync(1, request);

        result.MovementType.Should().Be("WITHDRAWAL");
        result.Value.Should().Be(-100m);
        _movementRepository.Verify(r => r.UpdateAsync(movement, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMovementDoesNotExist_ThrowsNotFoundException()
    {
        _movementRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Movement?)null);
        var request = new UpdateMovementRequest("WITHDRAWAL", -100m, 600m);

        var act = () => _sut.UpdateAsync(1, request);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
