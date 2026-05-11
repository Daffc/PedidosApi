namespace Domain.Exceptions;

public class PedidoClienteNomeInvalidoException : DomainException
{
    public PedidoClienteNomeInvalidoException() : base("Nome do cliente é obrigatório.")
    { }
}