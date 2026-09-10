namespace Devsu.PersonCustomer.Application.Dtos;

public record CustomerDto(
    long Id,
    string Name,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    bool Status);
