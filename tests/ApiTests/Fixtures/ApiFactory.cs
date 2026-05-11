using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;

namespace ApiTests.Fixtures;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            // REMOVE DEPENDENCIAS ORIGINAIS
            var descriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            var contextDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));

            if (contextDescriptor is not null)
                services.Remove(contextDescriptor);

            // ADICIONA BANCO EM MEMÓRIA
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.OpenConnection();
            db.Database.EnsureCreated();
        });
    }
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}