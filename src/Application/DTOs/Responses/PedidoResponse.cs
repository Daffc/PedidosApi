namespace Application.DTOs.Responses;

public sealed record PedidoResponse(
    Guid Id,
    string ClienteNome,
    DateTime DataCriacao,
    string Status,
    decimal ValorTotal,
    IReadOnlyCollection<PedidoItemResponse> Itens
);

public sealed record PedidoItemResponse (
    Guid Id,
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);