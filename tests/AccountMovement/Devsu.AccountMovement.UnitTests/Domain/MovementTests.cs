using Devsu.AccountMovement.Domain.Entities;
using Devsu.AccountMovement.Domain.Exceptions;
using FluentAssertions;

namespace Devsu.AccountMovement.UnitTests.Domain;

public class MovementTests
{
    private static Movement CreateValidMovement() =>
        new(movementDate: new DateTime(2026, 1, 22), movementType: "DEPOSIT", value: 600m, balance: 700m, accountId: 1);

    [Fact]
    public void Constructor_WithValidData_CreatesMovement()
    {
        var movement = CreateValidMovement();

        movement.MovementType.Should().Be("DEPOSIT");
        movement.Value.Should().Be(600m);
        movement.Balance.Should().Be(700m);
        movement.AccountId.Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithoutMovementType_ThrowsDomainException(string? movementType)
    {
        var act = () => new Movement(DateTime.UtcNow, movementType!, 600m, 700m, 1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithZeroValue_ThrowsDomainException()
    {
        var act = () => new Movement(DateTime.UtcNow, "DEPOSIT", 0m, 700m, 1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WithInvalidAccountId_ThrowsDomainException()
    {
        var act = () => new Movement(DateTime.UtcNow, "DEPOSIT", 600m, 700m, 0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithValidData_UpdatesMovement()
    {
        var movement = CreateValidMovement();

        movement.Update("WITHDRAWAL", -100m, 600m);

        movement.MovementType.Should().Be("WITHDRAWAL");
        movement.Value.Should().Be(-100m);
        movement.Balance.Should().Be(600m);
    }

    [Fact]
    public void Update_WithZeroValue_ThrowsDomainException()
    {
        var movement = CreateValidMovement();

        var act = () => movement.Update("WITHDRAWAL", 0m, 600m);

        act.Should().Throw<DomainException>();
    }
}
