namespace Application.DTOs.Responses;

public sealed record PagedResponse<T>(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IReadOnlyCollection<T> Items
);