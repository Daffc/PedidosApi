using FluentAssertions;

using Domain.Exceptions;

namespace UnitTests.Domain.Exceptions;

public class DomainExceptionsTests
{
    [Fact]
    public void PedidoSemItensException_DeveHerdarDeDomainException()
    {
        var exception = new PedidoSemItensException();

        exception.Should().BeOfType<PedidoSemItensException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PedidoSemItensException_DevePossuirMensagemPadrao()
    {
        var exception = new PedidoSemItensException();

        exception.Message.Should()
            .Be("O pedido deve possuir ao menos um item.");
    }

    [Fact]
    public void PedidoCancelamentoInvalidoException_DeveHerdarDeDomainException()
    {
        var exception = new PedidoCancelamentoInvalidoException();

        exception.Should().BeOfType<PedidoCancelamentoInvalidoException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PedidoCancelamentoInvalidoException_DevePossuirMensagemPadrao()
    {
        var exception = new PedidoCancelamentoInvalidoException();

        exception.Message.Should()
            .Be("Pedidos pagos não podem ser cancelados.");
    }

    [Fact]
    public void PedidoAdicionarItemInvalidoException_DeveHerdarDeDomainException()
    {
        var exception = new PedidoAdicionarItemInvalidoException();

        exception.Should().BeOfType<PedidoAdicionarItemInvalidoException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PedidoAdicionarItemInvalidoException_DevePossuirMensagemPadrao()
    {
        var exception = new PedidoAdicionarItemInvalidoException();

        exception.Message.Should()
            .Be("Item do pedido é obrigatório.");
    }

    [Fact]
    public void PedidoClienteNomeInvalidoException_DeveHerdarDeDomainException()
    {
        var exception = new PedidoClienteNomeInvalidoException();

        exception.Should().BeOfType<PedidoClienteNomeInvalidoException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PedidoClienteNomeInvalidoException_DevePossuirMensagemPadrao()
    {
        var exception = new PedidoClienteNomeInvalidoException();

        exception.Message.Should()
            .Be("Nome do cleinte é obrigatório.");
    }

    [Fact]
    public void PedidoPagamentoInvalidoException_DeveHerdarDeDomainException()
    {
        var exception = new PedidoPagamentoInvalidoException();

        exception.Should().BeOfType<PedidoPagamentoInvalidoException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PedidoPagamentoInvalidoException_DevePossuirMensagemPadrao()
    {
        var exception = new PedidoPagamentoInvalidoException();

        exception.Message.Should()
            .Be("Pedidos cancelados não podem ser pagos.");
    }

    [Fact]
    public void ItemPedidoProdutoNomeIvalidoException_DeveHerdarDeDomainException()
    {
        var exception = new ItemPedidoProdutoNomeIvalidoException();

        exception.Should().BeOfType<ItemPedidoProdutoNomeIvalidoException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ItemPedidoProdutoNomeIvalidoException_DevePossuirMensagemPadrao()
    {
        var exception = new ItemPedidoProdutoNomeIvalidoException();

        exception.Message.Should()
            .Be("Nome do produto de item inválido.");
    }

    [Fact]
    public void ItemPedidoPedidoIdIException_DeveHerdarDeDomainException()
    {
        var exception = new ItemPedidoPedidoIdIException();

        exception.Should().BeOfType<ItemPedidoPedidoIdIException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ItemPedidoPedidoIdIException_DevePossuirMensagemPadrao()
    {
        var exception = new ItemPedidoPedidoIdIException();

        exception.Message.Should()
            .Be("PedidoId inválido.");
    }

    [Fact]
    public void ItemPedidoQuantidadeInvalidaException_DeveHerdarDeDomainException()
    {
        var exception = new ItemPedidoQuantidadeInvalidaException();

        exception.Should().BeOfType<ItemPedidoQuantidadeInvalidaException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ItemPedidoQuantidadeInvalidaException_DevePossuirMensagemPadrao()
    {
        var exception = new ItemPedidoQuantidadeInvalidaException();

        exception.Message.Should()
            .Be("A quantidade de itens deve ser maior do que zero.");
    }

    [Fact]
    public void ItemPedidoPrecoUnitarioInvalidoException_DeveHerdarDeDomainException()
    {
        var exception = new ItemPedidoPrecoUnitarioInvalidoException();

        exception.Should().BeOfType<ItemPedidoPrecoUnitarioInvalidoException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ItemPedidoPrecoUnitarioInvalidoException_DevePossuirMensagemPadrao()
    {
        var exception = new ItemPedidoPrecoUnitarioInvalidoException();

        exception.Message.Should()
            .Be("O preço unitário deve ser maior do que zero.");
    }
}