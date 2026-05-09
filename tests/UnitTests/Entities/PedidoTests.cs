using FluentAssertions;

using Domain.Entities;
using Domain.Exceptions;
using Domain.Enums;

namespace UnitTests.Domain.Entities;

public class PedidoTests
{

    private static List<ItemPedido> CriarItensValidos()
    {
        return
        [
            new ItemPedido("Item1", 1, 3000m),
            new ItemPedido("Item2", 2, 100m)
        ];
    }

    [Fact]
    public void Deve_Criar_Pedido_Valido()
    {
        var itens = CriarItensValidos();

        var pedido = new Pedido(
            "Cliente",
            itens);

        pedido.ClienteNome.Should().Be("Cliente");
        pedido.Status.Should().Be(StatusPedido.Novo);
        pedido.Itens.Should().HaveCount(2);
    }

    [Fact]
    public void Deve_Gerar_Id_Automaticamente()
    {
        var itens = CriarItensValidos();

        var pedido = new Pedido(
            "Cliente",
            itens);

        pedido.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Deve_Definir_DataCriacao()
    {
        var antes = DateTime.UtcNow;

        var itens = CriarItensValidos();

        var pedido = new Pedido(
            "Cliente",
            itens);

        var depois = DateTime.UtcNow;

        pedido.DataCriacao.Should()
            .BeOnOrAfter(antes);

        pedido.DataCriacao.Should()
            .BeOnOrBefore(depois);
    }

    [Fact]
    public void Deve_Iniciar_Com_Status_Novo()
    {
        var itens = CriarItensValidos();

        var pedido = new Pedido(
            "Cliente",
            itens);

        pedido.Status.Should()
            .Be(StatusPedido.Novo);
    }

    [Fact]
    public void Deve_Calcular_ValorTotal_Corretamente()
    {
        var itens = new List<ItemPedido>
        {
            new("Notebook", 2, 3000m),
            new("Mouse", 3, 100m)
        };

        var pedido = new Pedido(
            "Cliente",
            itens);

        pedido.ValorTotal.Should().Be(6300m);
    }

    [Fact]
    public void Deve_Associar_PedidoId_Em_Todos_Itens()
    {
        var itens = CriarItensValidos();

        var pedido = new Pedido(
            "Cliente",
            itens);

        pedido.Itens.Should()
            .OnlyContain(i => i.PedidoId == pedido.Id);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_ClienteNome_For_Vazio()
    {
        var itens = CriarItensValidos();

        var action = () => new Pedido(
            string.Empty,
            itens);

        action.Should()
            .Throw<PedidoClienteNomeInvalidoException>();
    }


    [Fact]
    public void Deve_Lancar_Excecao_Quando_Itens_For_Nulo()
    {
        var itens = CriarItensValidos();

        var action = () => new Pedido(
            "Cliente",
            null!);

        action.Should()
            .Throw<PedidoSemItensException>();
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_ClienteNome_For_Espaco()
    {
        var itens = CriarItensValidos();

        var action = () => new Pedido(
            "   ",
            itens);

        action.Should()
            .Throw<PedidoClienteNomeInvalidoException>();
    }

    [Fact]
    public void Deve_Lancar_Excecao_Quando_Nao_Houver_Itens()
    {
        var itens = new List<ItemPedido>();

        var action = () => new Pedido(
            "Cliente",
            itens);

        action.Should()
            .Throw<PedidoSemItensException>();
    }

    [Fact]
    public void Deve_Adicionar_Item_Corretamente()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        var novoItem = new ItemPedido(
            "Teclado",
            1,
            250m);

        pedido.AdicionarItem(novoItem);

        pedido.Itens.Should().Contain(novoItem);
    }

    [Fact]
    public void Deve_Associar_PedidoId_Ao_Adicionar_Item()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        var novoItem = new ItemPedido(
            "Teclado",
            1,
            250m);

        pedido.AdicionarItem(novoItem);

        novoItem.PedidoId.Should().Be(pedido.Id);
    }

    [Fact]
    public void Deve_Recalcular_ValorTotal_Ao_Adicionar_Item()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        var novoItem = new ItemPedido(
            "Monitor",
            2,
            1000m);

        pedido.AdicionarItem(novoItem);

        pedido.ValorTotal.Should().Be(5200m);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Ao_Adicionar_Item_Nulo()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        var action = () => pedido.AdicionarItem(null!);

        action.Should()
            .Throw<PedidoAdicionarItemInvalidoException>();
    }

    [Fact]
    public void Deve_Lancar_Excecao_Ao_Adicionar_Itens_Nulos()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        var action = () => pedido.AdicionarItens(null!);

        action.Should()
            .Throw<PedidoSemItensException>();
    }

    [Fact]
    public void Deve_Cancelar_Pedido_Com_Status_Novo()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        pedido.Cancelar();

        pedido.Status.Should()
            .Be(StatusPedido.Cancelado);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Ao_Cancelar_Pedido_Pago()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        pedido.Pagar();

        var action = () => pedido.Cancelar();

        action.Should()
            .Throw<PedidoCancelamentoInvalidoException>();
    }

    [Fact]
    public void Deve_Marcar_Pedido_Como_Pago()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        pedido.Pagar();

        pedido.Status.Should()
            .Be(StatusPedido.Pago);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Ao_Pagar_Pedido_Cancelado()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        pedido.Cancelar();

        var action = () => pedido.Pagar();

        action.Should()
            .Throw<PedidoPagamentoInvalidoException>();
    }

    [Fact]
    public void Deve_Manter_Status_Cancelado_Ao_Cancelar_Duas_Vezes()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        pedido.Cancelar();

        pedido.Cancelar();

        pedido.Status.Should()
            .Be(StatusPedido.Cancelado);
    }

    [Fact]
    public void Deve_Manter_Status_Pago_Ao_Pagar_Duas_Vezes()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        pedido.Pagar();

        pedido.Pagar();

        pedido.Status.Should()
            .Be(StatusPedido.Pago);
    }

    [Fact]
    public void Itens_Deve_Ser_Somente_Leitura()
    {
        var pedido = new Pedido(
            "Cliente",
            CriarItensValidos());

        var action = () =>
        {
            ((List<ItemPedido>)pedido.Itens)
                .Add(new ItemPedido(
                    "Produto",
                    1,
                    10m));
        };

        action.Should()
            .Throw<InvalidCastException>();
    }
}