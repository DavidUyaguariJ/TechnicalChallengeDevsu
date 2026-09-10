using Devsu.AccountMovement.Application.Dtos;
using Devsu.AccountMovement.Application.Exceptions;
using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Application.Services;
using Devsu.AccountMovement.Domain.Entities;
using Devsu.Shared.Contracts;
using FluentAssertions;
using Moq;

namespace Devsu.AccountMovement.UnitTests.Application;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _repository = new();
    private readonly Mock<ICustomerServiceClient> _customerServiceClient = new();
    private readonly AccountService _sut;

    public AccountServiceTests()
    {
        _sut = new AccountService(_repository.Object, _customerServiceClient.Object);
        _customerServiceClient.Setup(c => c.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomerSummary(1, "Jose Lema", true));
    }

    private static Account CreateAccount() => new("478758", "Savings", 2000m, 1);

    [Fact]
    public async Task CreateAsync_WithNewAccountNumber_AddsAccountAndReturnsDto()
    {
        var request = new CreateAccountRequest("478758", "Savings", 2000m, 1);
        _repository.Setup(r => r.GetByAccountNumberAsync(request.AccountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        var result = await _sut.CreateAsync(request);

        result.AccountNumber.Should().Be("478758");
        result.Status.Should().BeTrue();
        _repository.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingAccountNumber_ThrowsConflictException()
    {
        var request = new CreateAccountRequest("478758", "Savings", 2000m, 1);
        _repository.Setup(r => r.GetByAccountNumberAsync(request.AccountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateAccount());

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<ConflictException>();
        _repository.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        var request = new CreateAccountRequest("478758", "Savings", 2000m, 99);
        _customerServiceClient.Setup(c => c.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerSummary?)null);

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<NotFoundException>();
        _repository.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAccountExists_ReturnsDto()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateAccount());

        var result = await _sut.GetByIdAsync(1);

        result.AccountNumber.Should().Be("478758");
    }

    [Fact]
    public async Task GetByIdAsync_WhenAccountDoesNotExist_ThrowsNotFoundException()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Account?)null);

        var act = () => _sut.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAccountsMapped()
    {
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account> { CreateAccount(), CreateAccount() });

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_WhenAccountExists_UpdatesAndReturnsDto()
    {
        var account = CreateAccount();
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(account);
        var request = new UpdateAccountRequest("Checking", false);

        var result = await _sut.UpdateAsync(1, request);

        result.AccountType.Should().Be("Checking");
        result.Status.Should().BeFalse();
        _repository.Verify(r => r.UpdateAsync(account, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenAccountDoesNotExist_ThrowsNotFoundException()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Account?)null);
        var request = new UpdateAccountRequest("Checking", false);

        var act = () => _sut.UpdateAsync(1, request);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
