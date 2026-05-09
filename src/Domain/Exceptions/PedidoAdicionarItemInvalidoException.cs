namespace Domain.Exceptions;

public class PedidoAdicionarItemInvalidoException : DomainException
{
    public PedidoAdicionarItemInvalidoException() : base("Item do pedido é obrigatório.")
    { }
}