using Devsu.PersonCustomer.Application.Dtos;
using Devsu.PersonCustomer.Application.Exceptions;
using Devsu.PersonCustomer.Application.Ports;
using Devsu.PersonCustomer.Application.Services;
using Devsu.PersonCustomer.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Devsu.PersonCustomer.UnitTests.Application;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(_repository.Object);
    }

    private static Customer CreateCustomer() =>
        new("Jose Lema", "Male", 35, "0102030405", "Otavalo sn y principal", "098254785", "1234");

    [Fact]
    public async Task CreateAsync_WithNewIdentification_AddsCustomerAndReturnsDto()
    {
        var request = new CreateCustomerRequest("Jose Lema", "Male", 35, "0102030405", "Otavalo sn y principal", "098254785", "1234");
        _repository.Setup(r => r.GetByIdentificationAsync(request.Identification, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _sut.CreateAsync(request);

        result.Name.Should().Be("Jose Lema");
        result.Identification.Should().Be("0102030405");
        result.Status.Should().BeTrue();
        _repository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingIdentification_ThrowsConflictException()
    {
        var request = new CreateCustomerRequest("Jose Lema", "Male", 35, "0102030405", "Otavalo sn y principal", "098254785", "1234");
        _repository.Setup(r => r.GetByIdentificationAsync(request.Identification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCustomer());

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<ConflictException>();
        _repository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsDto()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateCustomer());

        var result = await _sut.GetByIdAsync(1);

        result.Name.Should().Be("Jose Lema");
        result.Identification.Should().Be("0102030405");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

        var act = () => _sut.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomersMapped()
    {
        _repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer> { CreateCustomer(), CreateCustomer() });

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_UpdatesAndReturnsDto()
    {
        var customer = CreateCustomer();
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        var request = new UpdateCustomerRequest("Jose Andres Lema", "Male", 36, "Nueva direccion", "0999999999", "5678", false);

        var result = await _sut.UpdateAsync(1, request);

        result.Name.Should().Be("Jose Andres Lema");
        result.Address.Should().Be("Nueva direccion");
        result.Status.Should().BeFalse();
        _repository.Verify(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);
        var request = new UpdateCustomerRequest("Jose Andres Lema", "Male", 36, "Nueva direccion", "0999999999", null, true);

        var act = () => _sut.UpdateAsync(1, request);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_RemovesCustomer()
    {
        var customer = CreateCustomer();
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        await _sut.DeleteAsync(1);

        _repository.Verify(r => r.DeleteAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        _repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

        var act = () => _sut.DeleteAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
