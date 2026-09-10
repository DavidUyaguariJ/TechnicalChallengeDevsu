using Devsu.AccountMovement.Application.Ports;
using Devsu.AccountMovement.Infrastructure.ExternalServices;
using Devsu.AccountMovement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.AccountMovement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountMovementInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMovementRepository, MovementRepository>();

        var personCustomerBaseUrl = configuration["Services:PersonCustomer:BaseUrl"]
            ?? throw new InvalidOperationException("Services:PersonCustomer:BaseUrl configuration is required.");

        services.AddHttpClient<ICustomerServiceClient, CustomerServiceClient>(client =>
        {
            client.BaseAddress = new Uri(personCustomerBaseUrl.TrimEnd('/') + "/");
        });

        return services;
    }
}
