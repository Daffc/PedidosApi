using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Database;

public static class MigrationManager
{
    public static void ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            logger.LogInformation("Aplicando migrations do banco de dados...");

            context.Database.Migrate();

            logger.LogInformation("Migrations aplicadas com sucesso.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao aplicar migrations do banco de dados");
            throw;
        }
    }
}