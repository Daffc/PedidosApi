using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities;

public class ItemPedido : Entity
{
    public Guid PedidoId { get; private set; }
    public string ProdutoNome { get; private set; } = default!;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    protected ItemPedido()
    { }

    public ItemPedido(
        string produtoNome,
        int quantidade,
        decimal precoUnitario
    )
    {
        if (string.IsNullOrWhiteSpace(produtoNome))
            throw new ItemPedidoProdutoNomeIvalidoException();
        if (quantidade <= 0)
            throw new ItemPedidoQuantidadeInvalidaException();
        if (precoUnitario <= 0)
            throw new ItemPedidoPrecoUnitarioInvalidoException();

        ProdutoNome = produtoNome;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public void AssociarPedido(Guid pedidoId)
    {
        if (pedidoId == Guid.Empty)
            throw new ItemPedidoPedidoIdIException();

        PedidoId = pedidoId; ;
    }

    public void AtualizarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new ItemPedidoQuantidadeInvalidaException();

        Quantidade = quantidade;
    }

    public void AtualizarPrecoUnitario(decimal precoUnitario)
    {
        if (precoUnitario <= 0)
            throw new ItemPedidoPrecoUnitarioInvalidoException();

        PrecoUnitario = precoUnitario;
    }
}
