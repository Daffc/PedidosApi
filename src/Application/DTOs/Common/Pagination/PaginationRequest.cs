namespace Application.DTOs.Common.Pagination;

public abstract record PaginationRequest(
    int Page = 1,
    int PageSize = 10
);