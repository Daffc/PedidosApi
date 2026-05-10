using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // DbContext (SQLite)
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")
            )
        );

        // Repositories
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}