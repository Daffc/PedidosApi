using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Pedido : Entity
{
    private readonly List<ItemPedido> _itens = [];

    public string ClienteNome { get; private set; } = default!;
    public DateTime DataCriacao { get; private set; }
    public StatusPedido Status { get; private set; }

    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();
    public decimal ValorTotal => _itens.Sum(item => item.Subtotal);

    protected Pedido()
    {}

    public Pedido(
        string clienteNome,
        IEnumerable<ItemPedido> itens
    )
    {
        if (string.IsNullOrWhiteSpace(clienteNome))
            throw new PedidoClienteNomeInvalidoException();
        
        if (itens is null || !itens.Any())
            throw new PedidoSemItensException();

        ClienteNome = clienteNome.Trim();
        DataCriacao = DateTime.UtcNow;
        Status = StatusPedido.Novo;

        AdicionarItens(itens);
    }

    public void AdicionarItem(ItemPedido item)
    {
        if (item is null)
            throw new PedidoAdicionarItemInvalidoException();

        item.AssociarPedido(Id);

        _itens.Add(item);
    }

    public void AdicionarItens(IEnumerable<ItemPedido> itens)
    {
        if (itens is null || !itens.Any())
            throw new PedidoSemItensException();

        foreach (var item in itens)
        {
            AdicionarItem(item);
        }
    }

    public void Cancelar()
    {
        if (Status == StatusPedido.Pago)
            throw new PedidoCancelamentoInvalidoException();

        if (Status == StatusPedido.Cancelado)
            return;

        Status = StatusPedido.Cancelado;
    }

    public void Pagar()
    {
        if(Status == StatusPedido.Cancelado)
            throw new PedidoPagamentoInvalidoException();
        
        if (Status == StatusPedido.Pago)
            return;

        Status = StatusPedido.Pago;
    }
}