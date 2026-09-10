namespace Devsu.PersonCustomer.Application.Dtos;

public record CreateCustomerRequest(
    string Name,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    string Password);
