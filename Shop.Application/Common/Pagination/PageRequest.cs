namespace Shop.Application.Common.Pagination;

public record PageRequest(int Page = 1, int PageSize = 20);
