using Devsu.PersonCustomer.Domain.Exceptions;

namespace Devsu.PersonCustomer.Domain.Entities;

public sealed class Customer : Person
{
    public string Password { get; private set; } = default!;
    public bool Status { get; private set; }

    private Customer()
    {
    }

    public Customer(string name, string gender, int age, string identification, string address, string phone, string password)
        : base(name, gender, age, identification, address, phone)
    {
        SetPassword(password);
        Status = true;
    }

    public void UpdatePersonalInfo(string name, string gender, int age) => SetPersonalInfo(name, gender, age);

    public void UpdateContactInfo(string address, string phone) => SetContactInfo(address, phone);

    public void ChangePassword(string password) => SetPassword(password);

    public void Activate() => Status = true;

    public void Deactivate() => Status = false;

    private void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("Password is required.");
        }

        Password = password;
    }
}
