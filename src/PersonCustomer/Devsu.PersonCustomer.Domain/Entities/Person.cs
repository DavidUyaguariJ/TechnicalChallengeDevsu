using Devsu.PersonCustomer.Domain.Exceptions;

namespace Devsu.PersonCustomer.Domain.Entities;

public abstract class Person
{
    public long Id { get; protected set; }
    public string Name { get; protected set; } = default!;
    public string Gender { get; protected set; } = default!;
    public int Age { get; protected set; }
    public string Identification { get; protected set; } = default!;
    public string Address { get; protected set; } = default!;
    public string Phone { get; protected set; } = default!;

    protected Person()
    {
    }

    protected Person(string name, string gender, int age, string identification, string address, string phone)
    {
        SetPersonalInfo(name, gender, age);
        SetIdentification(identification);
        SetContactInfo(address, phone);
    }

    protected void SetPersonalInfo(string name, string gender, int age)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(gender))
        {
            throw new DomainException("Gender is required.");
        }

        if (age <= 0)
        {
            throw new DomainException("Age must be greater than zero.");
        }

        Name = name;
        Gender = gender;
        Age = age;
    }

    protected void SetContactInfo(string address, string phone)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new DomainException("Address is required.");
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainException("Phone is required.");
        }

        Address = address;
        Phone = phone;
    }

    private void SetIdentification(string identification)
    {
        if (string.IsNullOrWhiteSpace(identification))
        {
            throw new DomainException("Identification is required.");
        }

        Identification = identification;
    }
}
