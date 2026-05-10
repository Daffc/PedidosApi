using Microsoft.Extensions.DependencyInjection;

using Application.Interfaces.Services;
using Application.Services;

namespace Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPedidoService, PedidoService>();

        services.AddAutoMapper(configuration =>
        {
            configuration.AddMaps(typeof(PedidoService).Assembly);
        });
        
        return services;
    }
}