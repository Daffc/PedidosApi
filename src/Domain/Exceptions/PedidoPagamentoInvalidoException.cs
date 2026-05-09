namespace Domain.Exceptions;

public class PedidoPagamentoInvalidoException : DomainException
{
    public PedidoPagamentoInvalidoException() : base("Pedidos cancelados não podem ser pagos.")
    { }
}