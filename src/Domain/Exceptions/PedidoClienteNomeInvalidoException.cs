namespace Domain.Exceptions;

public class PedidoClienteNomeInvalidoException : DomainException
{
    public PedidoClienteNomeInvalidoException() : base("Nome do cleinte é obrigatório.")
    { }
}