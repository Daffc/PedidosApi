namespace Application.DTOs.Requests;

public sealed record CreatePedidoRequest(
    string ClienteNome,
    IReadOnlyCollection<PedidoItemRequest> Itens
);

public sealed record PedidoItemRequest(
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario
);
