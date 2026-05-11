namespace Application.DTOs.Requests;

public sealed record CreatePedidoRequest(
    string ClienteNome,
    IReadOnlyCollection<ItemPedidoRequest> Itens
);

public sealed record ItemPedidoRequest(
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario
);
