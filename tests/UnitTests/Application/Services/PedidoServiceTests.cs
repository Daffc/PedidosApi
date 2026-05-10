using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;

using Application.Common.Pagination;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces.Repositories;
using Application.Services;

namespace UnitTests.Application.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        _repositoryMock = new Mock<IPedidoRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new PedidoService(_repositoryMock.Object, _mapperMock.Object);
    }


    private static CreatePedidoRequest CriarCreatePedidoRequest() =>
        new(
            "Cliente",
            new[]
            {
                new PedidoItemRequest("Item1", 1, 100m),
                new PedidoItemRequest("Item2", 2, 50m)
            }
        );

    private static PedidoResponse CriarPedidoResponse(Pedido pedido) =>
        new(
            pedido.Id,
            pedido.ClienteNome,
            pedido.DataCriacao,
            pedido.Status.ToString(),
            pedido.ValorTotal,
            pedido.Itens.Select(item =>
                new PedidoItemResponse(
                    item.Id,
                    item.ProdutoNome,
                    item.Quantidade,
                    item.PrecoUnitario,
                    item.Subtotal
                )
            ).ToList()
        );

    [Fact]
    public async Task CreateAsync_Deve_Chamar_Repositorio_E_Retornar_Response()
    {
        var request = CriarCreatePedidoRequest();
        var pedidoCriado = default(Pedido?);

        _repositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(pedido => pedidoCriado = pedido)
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(mapper => mapper.Map<PedidoResponse>(It.IsAny<Pedido>()))
            .Returns((Pedido pedido) => CriarPedidoResponse(pedido));


        var result = await _service.CreateAsync(request);

        pedidoCriado.Should().NotBeNull();
        pedidoCriado.ClienteNome.Should().Be(request.ClienteNome);
        pedidoCriado.Itens.Should().HaveCount(request.Itens.Count);
        pedidoCriado.Itens.Should().Contain(i => i.ProdutoNome == "Item1" && i.Quantidade == 1 && i.PrecoUnitario == 100m);

        result.Should().BeEquivalentTo(CriarPedidoResponse(pedidoCriado));
        _repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Pedido>()), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<PedidoResponse>(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Retornar_Response_Quando_Pedido_Existir()
    {
        var pedido = new Pedido(
            "Cliente", 
            new[] { 
                new ItemPedido(
                    "Item", 
                    1, 
                    200m
                ) 
            }
        );

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(pedido.Id))
            .ReturnsAsync(pedido);

        _mapperMock
            .Setup(mapper => mapper.Map<PedidoResponse>(pedido))
            .Returns(CriarPedidoResponse(pedido));

        var result = await _service.GetByIdAsync(pedido.Id);

        result.Should().BeEquivalentTo(CriarPedidoResponse(pedido));
        _repositoryMock.Verify(repo => repo.GetByIdAsync(pedido.Id), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<PedidoResponse>(pedido), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Deve_Lancar_KeyNotFoundException_Quando_Pedido_Nao_Existir()
    {
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Pedido?)null);

        var id = Guid.NewGuid();

        var action = async () => await _service.GetByIdAsync(id);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Pedido '{id}' não encontrado.");

        _repositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Retornar_PagedResponse_Quando_StatusPedido_Nulo()
    {
        var pedidos = new[]
        {
            new Pedido("Cliente A", new[] { new ItemPedido("Produto 1", 1, 10m) }),
            new Pedido("Cliente B", new[] { new ItemPedido("Produto 2", 2, 20m) })
        };

        var pagedResult = new PagedResult<Pedido>(pedidos, 2, 1, 10);
        _repositoryMock
            .Setup(repo => repo.GetPagedAsync(null, 1, 10))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(mapper => mapper.Map<IReadOnlyCollection<PedidoResponse>>(It.IsAny<IEnumerable<Pedido>>()))
            .Returns((IEnumerable<Pedido> source) => source.Select(CriarPedidoResponse).ToList());

        var request = new GetPedidosRequest(null, 1, 10);

        var result = await _service.GetPagedAsync(request);

        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalItems.Should().Be(2);
        result.TotalPages.Should().Be(1);
        result.Items.Should().HaveCount(2);

        _repositoryMock.Verify(repo => repo.GetPagedAsync(null, 1, 10), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<IReadOnlyCollection<PedidoResponse>>(It.IsAny<IEnumerable<Pedido>>()), Times.Once);
    }

    [Theory]
    [InlineData("cancelado")]
    [InlineData("CANCELADO")]
    [InlineData("Cancelado")]
    public async Task GetPagedAsync_Deve_Converter_StatusPedido_Quando_String_For_Valida(string status)
    {
        var pedidos = new[]
        {
            new Pedido("Cliente C", new[] { new ItemPedido("Produto 3", 1, 30m) })
        };

        var pagedResult = new PagedResult<Pedido>(pedidos, 1, 1, 10);
        _repositoryMock
            .Setup(repo => repo.GetPagedAsync(StatusPedido.Cancelado, 1, 10))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(mapper => mapper.Map<IReadOnlyCollection<PedidoResponse>>(It.IsAny<IEnumerable<Pedido>>()))
            .Returns((IEnumerable<Pedido> source) => source.Select(CriarPedidoResponse).ToList());

        var request = new GetPedidosRequest(status, 1, 10);

        var result = await _service.GetPagedAsync(request);

        result.Items.Should().HaveCount(1);
        _repositoryMock.Verify(repo => repo.GetPagedAsync(StatusPedido.Cancelado, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_Deve_Lancar_ArgumentException_Quando_StatusPedido_For_Invalido()
    {
        var request = new GetPedidosRequest("invalido", 1, 10);

        var action = async () => await _service.GetPagedAsync(request);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("StatusPedido inválido.");
    }

    [Fact]
    public async Task CancelAsync_Deve_Cancelar_Pedido_Existente_E_Atualizar_Repositorio()
    {
        var pedido = new Pedido(
            "Cliente", 
            new[] { new ItemPedido("Item", 1, 250m) }
        );
        var pedidoAtualizado = default(Pedido?);

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(pedido.Id))
            .ReturnsAsync(pedido);
        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(p => pedidoAtualizado = p)
            .Returns(Task.CompletedTask);


        await _service.CancelAsync(pedido.Id);

        pedidoAtualizado.Should().NotBeNull();
        pedidoAtualizado!.Status.Should().Be(StatusPedido.Cancelado);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_Deve_Lancar_KeyNotFoundException_Quando_Pedido_Nao_Existir()
    {
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Pedido?)null);

        var id = Guid.NewGuid();

        var action = async () => await _service.CancelAsync(id);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Pedido '{id}' não encontrado.");

        _repositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
    }
}
