using Devsu.AccountMovement.Application.Ports;
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

        return services;
    }
}
