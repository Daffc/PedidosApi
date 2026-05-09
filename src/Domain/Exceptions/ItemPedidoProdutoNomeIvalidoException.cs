namespace Domain.Exceptions;

public class ItemPedidoProdutoNomeIvalidoException : DomainException
{
    public ItemPedidoProdutoNomeIvalidoException() : base("Nome do produto de item inválido.")
    { }
}