using FluentAssertions;

using Domain.Entities;
using Domain.Exceptions;

namespace UnitTests.Domain.Entities;

public class ItemPedidoTests
{
    [Fact]
    public void Deve_Criar_ItemPedido_Valido()
    {
        var item = new ItemPedido(
            "Item",
            2,
            3500.50m);

        item.ProdutoNome.Should().Be("Item");
        item.Quantidade.Should().Be(2);
        item.PrecoUnitario.Should().Be(3500.50m);
    }

    [Fact]
    public void Deve_Calcular_Subtotal_Corretamente()
    {
        var item = new ItemPedido(
            "Item",
            3,
            100m);

        var subtotal = item.Subtotal;

        subtotal.Should().Be(300m);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_ItemNome_For_Vazio()
    {
        var action = () => new ItemPedido(
            string.Empty,
            1,
            100m);

        action.Should()
            .Throw<ItemPedidoProdutoNomeIvalidoException>();
    }


    [Fact]
    public void Deve_Lancar_Excecao_Quando_ItemNome_For_Espaco()
    {
        var action = () => new ItemPedido(
            "   ",
            1,
            100m);

        action.Should()
            .Throw<ItemPedidoProdutoNomeIvalidoException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Lancar_Excecao_Quando_Quantidade_Invalida(int input)
    {
        var action = () => new ItemPedido(
            "Item",
            input,
            100m);

        action.Should()
            .Throw<ItemPedidoQuantidadeInvalidaException>();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Deve_Lancar_Excecao_Quando_PrecoUnitario_Invalido(decimal input)
    {
        var action = () => new ItemPedido(
            "Item",
            1,
            input);

        action.Should()
            .Throw<ItemPedidoPrecoUnitarioInvalidoException>();
    }

    [Fact]
    public void Deve_Associar_PedidoId_Corretamente()
    {
        var item = new ItemPedido(
            "Item",
            1,
            1500m);

        var pedidoId = Guid.NewGuid();

        item.AssociarPedido(pedidoId);

        item.PedidoId.Should().Be(pedidoId);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Associar_PedidoId_Vazio()
    {
        var item = new ItemPedido(
            "Item",
            1,
            1500m);

        var action = () => item.AssociarPedido(Guid.Empty);

        action.Should()
            .Throw<ItemPedidoPedidoIdIException>();
    }

    [Fact]
    public void Deve_Atualizar_Quantidade_Corretamente()
    {
        var item = new ItemPedido(
            "Item",
            1,
            800m);

        item.AtualizarQuantidade(5);

        item.Quantidade.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Lancar_Excecao_Ao_Atualizar_Quantidade_Invalida(int input)
    {
        var item = new ItemPedido(
            "Item",
            1,
            800m);

        var action = () => item.AtualizarQuantidade(input);

        action.Should()
            .Throw<ItemPedidoQuantidadeInvalidaException>();
    }

    [Fact]
    public void Deve_Atualizar_Preco_Corretamente()
    {
        var item = new ItemPedido(
            "Item",
            1,
            500m);

        item.AtualizarPrecoUnitario(750m);

        item.PrecoUnitario.Should().Be(750m);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Deve_Lancar_Excecao_Ao_Atualizar_Preco_Invalido(decimal input)
    {
        var item = new ItemPedido(
            "Item",
            1,
            500m);

        var action = () => item.AtualizarPrecoUnitario(input);

        action.Should()
            .Throw<ItemPedidoPrecoUnitarioInvalidoException>();
    }

    [Fact]
    public void Deve_Recalcular_Subtotal_Apos_Atualizar_Quantidade()
    {
        var item = new ItemPedido(
            "Item",
            2,
            200m);

        item.AtualizarQuantidade(5);

        item.Subtotal.Should().Be(1000m);
    }

    [Fact]
    public void Deve_Recalcular_Subtotal_Apos_Atualizar_Preco()
    {
        var item = new ItemPedido(
            "Item",
            2,
            150m);

        item.AtualizarPrecoUnitario(300m);

        item.Subtotal.Should().Be(600m);
    }
}