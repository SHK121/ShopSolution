using Shop.Application.Common.Pagination;

namespace Shop.Application.Order.Queries;

public record GetOrderListQuery(Guid UserId, PageRequest Pagination);
