using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using IntegrationTests.Infrastructure.Persistence;




namespace IntegrationTests.Infrastructure.Persistence.Configurations;

public class PedidoConfigurationTests
{
    [Fact]
    public void Deve_Mapear_Tabela_Corretamente()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var entityType = dbContext.Model
            .FindEntityType(typeof(Pedido));

        entityType!
            .GetTableName()
            .Should()
            .Be("Pedidos");
    }

    [Fact]
    public void Deve_Configurar_Chave_Primaria()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var primaryKey = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindPrimaryKey();

        primaryKey.Should().NotBeNull();

        primaryKey!.Properties.Should()
            .ContainSingle(p => p.Name == "Id");
    }

    [Fact]
    public void Deve_Configurar_Id_Com_ValueGeneratedNever()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindProperty(nameof(Pedido.Id));

        property!.ValueGenerated
            .Should()
            .Be(ValueGenerated.Never);
    }

    [Fact]
    public void Deve_Configurar_ClienteNome_Com_MaxLength_200()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindProperty(nameof(Pedido.ClienteNome));

        property!.GetMaxLength()
            .Should()
            .Be(200);
    }

    [Fact]
    public void Deve_Configurar_ClienteNome_Como_Obrigatorio()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindProperty(nameof(Pedido.ClienteNome));

        property!.IsNullable
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Deve_Configurar_Status_Como_String()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindProperty(nameof(Pedido.Status));

        property!.GetColumnType()
            .Should()
            .Contain("TEXT");
    }

    [Fact]
    public void Deve_Configurar_Status_Com_MaxLength_50()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindProperty(nameof(Pedido.Status));

        property!.GetMaxLength()
            .Should()
            .Be(20);
    }

    [Fact]
    public void Deve_Configurar_Relacionamento_Com_ItemPedido()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var navigation = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindNavigation(nameof(Pedido.Itens));

        navigation.Should().NotBeNull();

        navigation!.ForeignKey.DeleteBehavior
            .Should()
            .Be(DeleteBehavior.Cascade);
    }

    [Fact]
    public void Deve_Configurar_BackField_Itens()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var navigation = dbContext.Model
            .FindEntityType(typeof(Pedido))!
            .FindNavigation(nameof(Pedido.Itens));

        navigation!.GetPropertyAccessMode()
            .Should()
            .Be(PropertyAccessMode.Field);
    }

    [Fact]
    public async Task Deve_Persistir_Pedido_Com_Itens()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var pedido = new Pedido(
            "Cliente",
            [
                new ItemPedido("Item1", 1, 5000m),
                new ItemPedido("Item2", 2, 100m)
            ]);

        dbContext.Add(pedido);

        await dbContext.SaveChangesAsync();

        var persisted = await dbContext
            .Set<Pedido>()
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == pedido.Id);

        persisted.Should().NotBeNull();

        persisted!.Itens.Should().HaveCount(2);

        persisted.ClienteNome.Should().Be("Cliente");
    }

    [Fact]
    public async Task Deve_Persistir_Status_Como_String()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var pedido = new Pedido(
            "Cliente",
            [
                new ItemPedido("Item", 1, 5000m)
            ]);

        pedido.Pagar();

        dbContext.Add(pedido);

        await dbContext.SaveChangesAsync();

        var status = await dbContext.Database
        .SqlQueryRaw<string>(
            """
            SELECT Status AS Value
            FROM Pedidos
            WHERE Id = {0}
            """,
            pedido.Id)
        .FirstAsync();

        status.Should().Be("Pago");
    }

    [Fact]
    public async Task Deve_Aplicar_CascadeDelete_Para_Itens()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var pedido = new Pedido(
            "Cliente",
            [
                new ItemPedido("Item1", 1, 5000m),
                new ItemPedido("Item2", 2, 100m)
            ]);

        dbContext.Add(pedido);

        await dbContext.SaveChangesAsync();

        dbContext.Remove(pedido);

        await dbContext.SaveChangesAsync();

        var itens = await dbContext
            .Set<ItemPedido>()
            .Where(i => i.PedidoId == pedido.Id)
            .ToListAsync();

        itens.Should().BeEmpty();
    }

    [Fact]
    public async Task Deve_Materializar_Agregado_Corretamente()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var pedido = new Pedido(
            "Cliente",
            [
                new ItemPedido("Item1", 1, 5000m),
                new ItemPedido("Item2", 2, 100m)
            ]);

        dbContext.Add(pedido);

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        var persisted = await dbContext
            .Set<Pedido>()
            .Include(p => p.Itens)
            .FirstAsync(p => p.Id == pedido.Id);

        persisted.Itens.Should().HaveCount(2);

        persisted.ValorTotal.Should().Be(5200m);

        persisted.Status.Should().Be(StatusPedido.Novo);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_ClienteNome_Maior_Que_200()
    {
        using var dbContext = AppDbContextTestFactory.Create();
        
        var sql = @"
            INSERT INTO Pedidos
            (
                Id,
                ClienteNome,
                DataCriacao,
                Status,
            )
            VALUES
            (
                $id,
                $clienteNome,
                datetime('now'),
                'Pago'
            );";

        var action = async () =>
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                sql,
                new Microsoft.Data.Sqlite.SqliteParameter("$id", Guid.NewGuid()),
                new Microsoft.Data.Sqlite.SqliteParameter("$clienteNome", new String('*', 201))
            );
        };

        await action.Should().ThrowAsync<Microsoft.Data.Sqlite.SqliteException>();
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Status_Maior_Que_20()
    {
        using var dbContext = AppDbContextTestFactory.Create();
        
        var sql = @"
            INSERT INTO Pedidos
            (
                Id,
                ClienteNome,
                DataCriacao,
                Status
            )
            VALUES
            (
                $id,
                'Cliente',
                datetime('now'),
                $status
            );";

        var action = async() => {
            await dbContext.Database.ExecuteSqlRawAsync(
                sql,
                new Microsoft.Data.Sqlite.SqliteParameter("$id", Guid.NewGuid()),
                new Microsoft.Data.Sqlite.SqliteParameter("$status", new String('*', 21))
            );
        };

        await action.Should().ThrowAsync<Microsoft.Data.Sqlite.SqliteException>();
    }
}