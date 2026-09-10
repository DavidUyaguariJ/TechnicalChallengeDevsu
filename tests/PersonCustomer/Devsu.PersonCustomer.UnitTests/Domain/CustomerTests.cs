using Devsu.PersonCustomer.Domain.Entities;
using Devsu.PersonCustomer.Domain.Exceptions;
using FluentAssertions;

namespace Devsu.PersonCustomer.UnitTests.Domain;

public class CustomerTests
{
    private static Customer CreateValidCustomer() =>
        new(
            name: "Jose Lema",
            gender: "Male",
            age: 35,
            identification: "0102030405",
            address: "Otavalo sn y principal",
            phone: "098254785",
            password: "1234");

    [Fact]
    public void Constructor_WithValidData_CreatesActiveCustomer()
    {
        var customer = CreateValidCustomer();

        customer.Name.Should().Be("Jose Lema");
        customer.Gender.Should().Be("Male");
        customer.Age.Should().Be(35);
        customer.Identification.Should().Be("0102030405");
        customer.Address.Should().Be("Otavalo sn y principal");
        customer.Phone.Should().Be("098254785");
        customer.Password.Should().Be("1234");
        customer.Status.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithoutName_ThrowsDomainException(string? name)
    {
        var act = () => new Customer(name!, "Male", 35, "0102030405", "Otavalo sn y principal", "098254785", "1234");

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidAge_ThrowsDomainException(int age)
    {
        var act = () => new Customer("Jose Lema", "Male", age, "0102030405", "Otavalo sn y principal", "098254785", "1234");

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithoutIdentification_ThrowsDomainException(string? identification)
    {
        var act = () => new Customer("Jose Lema", "Male", 35, identification!, "Otavalo sn y principal", "098254785", "1234");

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithoutPassword_ThrowsDomainException(string? password)
    {
        var act = () => new Customer("Jose Lema", "Male", 35, "0102030405", "Otavalo sn y principal", "098254785", password!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdatePersonalInfo_WithValidData_UpdatesNameGenderAndAge()
    {
        var customer = CreateValidCustomer();

        customer.UpdatePersonalInfo("Jose Andres Lema", "Male", 36);

        customer.Name.Should().Be("Jose Andres Lema");
        customer.Age.Should().Be(36);
    }

    [Fact]
    public void UpdateContactInfo_WithValidData_UpdatesAddressAndPhone()
    {
        var customer = CreateValidCustomer();

        customer.UpdateContactInfo("Nueva direccion", "0999999999");

        customer.Address.Should().Be("Nueva direccion");
        customer.Phone.Should().Be("0999999999");
    }

    [Fact]
    public void UpdateContactInfo_WithEmptyAddress_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();

        var act = () => customer.UpdateContactInfo(" ", "0999999999");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ChangePassword_WithValidPassword_UpdatesPassword()
    {
        var customer = CreateValidCustomer();

        customer.ChangePassword("5678");

        customer.Password.Should().Be("5678");
    }

    [Fact]
    public void ChangePassword_WithEmptyPassword_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();

        var act = () => customer.ChangePassword(" ");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deactivate_SetsStatusToFalse()
    {
        var customer = CreateValidCustomer();

        customer.Deactivate();

        customer.Status.Should().BeFalse();
    }

    [Fact]
    public void Activate_SetsStatusToTrue()
    {
        var customer = CreateValidCustomer();
        customer.Deactivate();

        customer.Activate();

        customer.Status.Should().BeTrue();
    }
}
