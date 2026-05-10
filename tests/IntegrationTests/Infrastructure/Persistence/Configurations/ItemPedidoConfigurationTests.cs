using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain.Entities;
using Infrastructure.Persistence;
using IntegrationTests.Infrastructure.Persistence;

namespace IntegrationTests.Infrastructure.Persistence.Configurations;

public class ItemPedidoConfigurationTests
{
    [Fact]
    public void Deve_Mapear_Tabela_Corretamente()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(ItemPedido));

        entityType!.GetTableName()
            .Should()
            .Be("ItensPedido");
    }

    [Fact]
    public void Deve_Configurar_Chave_Primaria()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var entityType = dbContext.Model.FindEntityType(typeof(ItemPedido));

        var primaryKey = entityType!
            .FindPrimaryKey();

        primaryKey.Should().NotBeNull();

        primaryKey!.Properties
            .Should()
            .ContainSingle(p => p.Name == "Id");
    }

    [Fact]
    public void Deve_Configurar_ProdutoNome_Com_MaxLength_200()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(ItemPedido))!
            .FindProperty(nameof(ItemPedido.ProdutoNome));

        property!.GetMaxLength()
            .Should()
            .Be(200);
    }

    [Fact]
    public void Deve_Configurar_ProdutoNome_Como_Obrigatorio()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(ItemPedido))!
            .FindProperty(nameof(ItemPedido.ProdutoNome));

        property!.IsNullable
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Deve_Configurar_PrecoUnitario_Com_Precisao_18_2()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(ItemPedido))!
            .FindProperty(nameof(ItemPedido.PrecoUnitario));

        property!.GetPrecision()
            .Should()
            .Be(18);

        property.GetScale()
            .Should()
            .Be(2);
    }

    [Fact]
    public void Deve_Ignorar_Propriedade_Subtotal()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var property = dbContext.Model
            .FindEntityType(typeof(ItemPedido))!
            .FindProperty(nameof(ItemPedido.Subtotal));

        property.Should().BeNull();
    }

    [Fact]
    public async Task Deve_Persistir_ItemPedido_Valido()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var item = new ItemPedido(
            "Item",
            2,
            5000m);

        var pedido = new Pedido(
            "Cliente", 
            [item]
        );

        item.AssociarPedido(pedido.Id);

        dbContext.Add(pedido);
        dbContext.Add(item);

        await dbContext.SaveChangesAsync();

        var persisted = await dbContext
            .Set<ItemPedido>()
            .FirstOrDefaultAsync(i => i.Id == item.Id);

        persisted.Should().NotBeNull();

        persisted!.ProdutoNome.Should().Be("Item");
    }

    [Fact]
    public async Task Deve_Aplicar_CheckConstraint_Quantidade()
    {
        using var dbContext = AppDbContextTestFactory.Create();

        var pedido = new Pedido(
            "Cliente",
            [
                new ItemPedido("Item", 1, 10m)
            ]
        );

        dbContext.Add(pedido);
        await dbContext.SaveChangesAsync();

        var sql = @"
            INSERT INTO ItensPedido
            (
                Id,
                PedidoId,
                ProdutoNome,
                Quantidade,
                PrecoUnitario
            )
            VALUES
            (
                $id,
                $pedidoId,
                'Produto',
                0,
                10
            );";

        var action = async () =>
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                sql,
                new Microsoft.Data.Sqlite.SqliteParameter("$id", Guid.NewGuid()),
                new Microsoft.Data.Sqlite.SqliteParameter("$pedidoId", pedido.Id));
        };

        await action.Should().ThrowAsync<Microsoft.Data.Sqlite.SqliteException>();
    }

    [Fact]
    public async Task Deve_Aplicar_CheckConstraint_PrecoUnitario()
    {
        using var dbContext = AppDbContextTestFactory.Create();
        
        var pedido = new Pedido(
            "Cliente",
            [
                new ItemPedido("Item", 1, 10m)
            ]
        );

        dbContext.Add(pedido);
        await dbContext.SaveChangesAsync();

        var sql = @"
            INSERT INTO ItensPedido
            (
                Id,
                PedidoId,
                ProdutoNome,
                Quantidade,
                PrecoUnitario
            )
            VALUES
            (
                $id,
                $pedidoId,
                'Produto',
                1,
                0
            );";

        var action = async () =>
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                sql,
                new Microsoft.Data.Sqlite.SqliteParameter("$id", Guid.NewGuid()),
                new Microsoft.Data.Sqlite.SqliteParameter("$pedidoId", pedido.Id));
        };

        await action.Should().ThrowAsync<Microsoft.Data.Sqlite.SqliteException>();
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_ProdutoNome_Maior_Que_200()
    {
        using var dbContext = AppDbContextTestFactory.Create();
        
        var pedido = new Pedido(
            "Cliente",
            [
                new ItemPedido("Item", 1, 10m)
            ]
        );

        dbContext.Add(pedido);
        await dbContext.SaveChangesAsync();

        var sql = @"
            INSERT INTO ItensPedido
            (
                Id,
                PedidoId,
                ProdutoNome,
                Quantidade,
                PrecoUnitario
            )
            VALUES
            (
                $id,
                $pedidoId,
                $produtoNome,
                1,
                10
            );";

        var action = async () =>
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                sql,
                new Microsoft.Data.Sqlite.SqliteParameter("$id", Guid.NewGuid()),
                new Microsoft.Data.Sqlite.SqliteParameter("$produtoNome", new String('*', 201)),
                new Microsoft.Data.Sqlite.SqliteParameter("$pedidoId", pedido.Id));
        };

        await action.Should().ThrowAsync<Microsoft.Data.Sqlite.SqliteException>();
    }
}