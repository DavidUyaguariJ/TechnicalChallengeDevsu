using Devsu.PersonCustomer.Application.Ports;
using Devsu.PersonCustomer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.PersonCustomer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPersonCustomerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
