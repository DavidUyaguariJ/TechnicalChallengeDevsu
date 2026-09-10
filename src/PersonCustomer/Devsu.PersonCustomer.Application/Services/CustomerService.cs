using Devsu.PersonCustomer.Application.Dtos;
using Devsu.PersonCustomer.Application.Exceptions;
using Devsu.PersonCustomer.Application.Ports;
using Devsu.PersonCustomer.Domain.Entities;

namespace Devsu.PersonCustomer.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(ToDto).ToList();
    }

    public async Task<CustomerDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var customer = await GetExistingCustomerAsync(id, cancellationToken);
        return ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var existingCustomer = await _customerRepository.GetByIdentificationAsync(request.Identification, cancellationToken);
        if (existingCustomer is not null)
        {
            throw new ConflictException($"A customer with identification '{request.Identification}' already exists.");
        }

        var customer = new Customer(
            request.Name,
            request.Gender,
            request.Age,
            request.Identification,
            request.Address,
            request.Phone,
            request.Password);

        await _customerRepository.AddAsync(customer, cancellationToken);

        return ToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(long id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await GetExistingCustomerAsync(id, cancellationToken);

        customer.UpdatePersonalInfo(request.Name, request.Gender, request.Age);
        customer.UpdateContactInfo(request.Address, request.Phone);

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            customer.ChangePassword(request.Password);
        }

        if (request.Status)
        {
            customer.Activate();
        }
        else
        {
            customer.Deactivate();
        }

        await _customerRepository.UpdateAsync(customer, cancellationToken);

        return ToDto(customer);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var customer = await GetExistingCustomerAsync(id, cancellationToken);
        await _customerRepository.DeleteAsync(customer, cancellationToken);
    }

    private async Task<Customer> GetExistingCustomerAsync(long id, CancellationToken cancellationToken)
    {
        return await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Customer '{id}' was not found.");
    }

    private static CustomerDto ToDto(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Gender,
        customer.Age,
        customer.Identification,
        customer.Address,
        customer.Phone,
        customer.Status);
}
