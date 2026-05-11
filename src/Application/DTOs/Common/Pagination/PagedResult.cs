namespace Application.DTOs.Common.Pagination;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int TotalItems,
    int Page,
    int PageSize
)
{
    public int TotalPages =>
        (int)Math.Ceiling((double)TotalItems / PageSize);
}