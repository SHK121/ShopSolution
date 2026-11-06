namespace Shop.Api.Contracts;

public class PaginatedResponse<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }

    public PaginatedResponse(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = pageSize > 0
            ? (int)Math.Ceiling((double)totalCount / pageSize)
            : 0;
    }
}
