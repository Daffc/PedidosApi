namespace Domain.Exceptions;

public class ItemPedidoQuantidadeInvalidaException : DomainException
{
    public ItemPedidoQuantidadeInvalidaException() : base("A quantidade de itens deve ser maior do que zero.")
    { }
}