using Microsoft.EntityFrameworkCore;

using Domain.Entities;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItemPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );
        
        base.OnModelCreating(modelBuilder);
    }
}