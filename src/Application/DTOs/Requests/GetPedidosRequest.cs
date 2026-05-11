using Application.DTOs.Common.Pagination;

namespace Application.DTOs.Requests;

public sealed record GetPedidosRequest(
    string? StatusPedido,
    int Page = 1,
    int PageSize = 10
) : PaginationRequest(Page, PageSize);