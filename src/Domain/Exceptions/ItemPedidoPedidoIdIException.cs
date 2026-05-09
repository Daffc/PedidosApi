namespace Domain.Exceptions;

public class ItemPedidoPedidoIdIException : DomainException
{
    public ItemPedidoPedidoIdIException() : base("PedidoId inválido.")
    { }
}