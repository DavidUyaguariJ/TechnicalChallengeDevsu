namespace Devsu.PersonCustomer.Application.Dtos;

public record UpdateCustomerRequest(
    string Name,
    string Gender,
    int Age,
    string Address,
    string Phone,
    string? Password,
    bool Status);
