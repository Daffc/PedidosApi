using AutoMapper;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

using Application.DTOs.Requests;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Domain.Entities;
using Domain.Enums;

namespace IntegrationTests.Application;

public class PedidoServiceIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly PedidoService _service;

    public PedidoServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        var repository = new PedidoRepository(_context);

        var mapperConfig = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(PedidoService).Assembly),
            NullLoggerFactory.Instance
        );

        var mapper = mapperConfig.CreateMapper();

        _service = new PedidoService(repository, mapper);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_Deve_PersistirPedido_Em_Database()
    {
        var request = new CreatePedidoRequest(
            "Cliente",
            new List<ItemPedidoRequest>
            {
                new ("Item1", 2, 10),
                new ("Item2", 1, 20)
            }
        );

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        result.ClienteNome.Should().Be("Cliente");

        var dbPedido = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == result.Id);

        dbPedido.Should().NotBeNull();
        dbPedido!.Itens.Should().HaveCount(2);
        dbPedido.ValorTotal.Should().Be(40);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_Pedido_Completo()
    {
        
        var pedido = new Pedido(
            "Cliente",
            new List<ItemPedido>
            {
                new ItemPedido("Item1", 2, 10),
                new ItemPedido("Item2", 1, 20)
            }
        );

        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(pedido.Id);

        result.Should().NotBeNull();
        result.Itens.Should().HaveCount(2);
        result.ValorTotal.Should().Be(40);
    }

    [Fact]
    public async Task CancelAsync_Deve_Persistir_Troca_StatusChange()
    {
        var pedido = new Pedido(
            "Cliente",
            new List<ItemPedido>
            {
                new ItemPedido("Item", 1, 10)
            }
        );

        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();

        await _service.CancelAsync(pedido.Id);

        var dbPedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == pedido.Id);

        dbPedido.Should().NotBeNull();
        dbPedido!.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Retornar_PagedResults()
    {
        for (int i = 0; i < 15; i++)
        {
            await _context.Pedidos.AddAsync(
                new Pedido(
                    $"Cliente {i}",
                    new List<ItemPedido>
                    {
                        new ItemPedido("Item", 1, 10)
                    }
                )
            );
        }

        await _context.SaveChangesAsync();

        var request = new GetPedidosRequest(
            null,
            1,
            10
        );

        var result = await _service.GetPagedAsync(request);

        result.Items.Should().HaveCount(10);
        result.TotalItems.Should().Be(15);
        result.TotalPages.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Filtrar_Por_Status()
    {
        var pedidoNovo = new Pedido(
            "Cliente",
            new List<ItemPedido>
            {
                new ItemPedido("Item", 1, 10)
            }
        );
        
        var pedidoPago = new Pedido(
            "Cliente",
            new List<ItemPedido>
            {
                new ItemPedido("Item", 1, 10)
            }
        );

        pedidoPago.Pagar();

        await _context.Pedidos.AddAsync(pedidoNovo);
        await _context.Pedidos.AddAsync(pedidoPago);
        await _context.SaveChangesAsync();

        var request = new GetPedidosRequest(
            "Pago",
            1,
            10
        );

        var result = await _service.GetPagedAsync(request);

        result.Items.Should().HaveCount(1);
        result.TotalItems.Should().Be(1);
        result.Items.Should().AllSatisfy(item => 
            item.Status.Should().Be("Pago")
        );
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Retornar_PagedResult_Quando_Sem_Corresponsdencia()
    {
        var request = new GetPedidosRequest(
            "Cancelado",
            1,
            10
        );

        var result = await _service.GetPagedAsync(request);

        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_Deve_CalcularTotal()
    {
        var request = new CreatePedidoRequest(
            "Cliente",
            new List<ItemPedidoRequest>
            {
                new("Item 1", 3, 15.50m),
                new("Item 2", 2, 7.25m),
                new("Item 3", 1, 100.00m)
            }
        );

        var result = await _service.CreateAsync(request);

        var expectedTotal = (3 * 15.50m) + (2 * 7.25m) + 100.00m;
        
        result.ValorTotal.Should().Be(expectedTotal);
        result.Itens.Should().HaveCount(3);
        
        var dbPedido = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == result.Id);

        dbPedido.Should().NotBeNull();
        dbPedido!.ValorTotal.Should().Be(expectedTotal);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Lancar_KeyNotFoundException_Quando_Pedido_Nao_Existir()
    {
        var nonExistentId = Guid.NewGuid();

        var action = async () => await _service.GetByIdAsync(nonExistentId);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Pedido '{nonExistentId}' não encontrado.");
    }

    [Fact]
    public async Task CancelAsync_Deve_Lancar_KeyNotFoundException_Quando_Pedido_Nao_Existir()
    {
        var nonExistentId = Guid.NewGuid();

        var action = async () => await _service.CancelAsync(nonExistentId);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Pedido '{nonExistentId}' não encontrado.");
    }
}