using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Infrastructure.Persistence;

namespace IntegrationTests.Infrastructure.Persistence;

public static class AppDbContextTestFactory
{
    public static AppDbContext Create()
    {
        var connection = new SqliteConnection("Data Source=:memory:");

        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new AppDbContext(options);

        dbContext.Database.OpenConnection();
        
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}