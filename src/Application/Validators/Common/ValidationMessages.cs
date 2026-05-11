namespace Application.Validators.Common;

public static class ValidationMessages
{
    public const string RequiredField = "Campo obrigatório.";
    public const string InvalidPage = "Página deve ser maior que zero.";
    public const string InvalidPageSize = "Tamanho da página deve ser maior que zero.";
    public const string EmptyPedidoItens = "Pedido deve possuir ao menos um item.";
    public const string InvalidItemPedidoQuantidade = "Quantidade deve ser maior que zero.";
    public const string InvalidItemPedidoPrecoUnitario = "Preço unitário deve ser maior que zero.";
    public const string InvalidPedidoStatus = "Status inválido.";
    public const string PedidoClienteNomeMaxLength = "Nome do cliente deve possuir no máximo 200 caracteres.";
    public const string ItemPedidoProdutoNomeMaxLength = "Nome do produto deve possuir no máximo 200 caracteres.";
}