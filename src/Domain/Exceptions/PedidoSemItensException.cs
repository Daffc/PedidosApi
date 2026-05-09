namespace Domain.Exceptions;

public class PedidoSemItensException : DomainException
{
    public PedidoSemItensException() : base("O pedido deve possuir ao menos um item.")
    { }
}