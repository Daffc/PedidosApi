namespace Domain.Exceptions;

public class ItemPedidoPrecoUnitarioInvalidoException : DomainException
{
    public ItemPedidoPrecoUnitarioInvalidoException() : base("O preço unitário deve ser maior do que zero.")
    { }
}