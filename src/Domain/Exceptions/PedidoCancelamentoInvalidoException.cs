namespace Domain.Exceptions;

public class PedidoCancelamentoInvalidoException : DomainException
{
    public PedidoCancelamentoInvalidoException() : base("Pedidos pagos não podem ser cancelados.")
    { }
}