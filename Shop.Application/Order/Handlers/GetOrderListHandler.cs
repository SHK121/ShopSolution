using Shop.Application.Interfaces;
using Shop.Application.Order.Queries;
using Shop.Application.Common.Pagination;
using OrderEntity = Shop.Domain.Entities.Order;

namespace Shop.Application.Order.Handlers;

public class GetOrderListHandler
{
    private readonly IOrderRepository _repository;

    public GetOrderListHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResult<OrderEntity>> Handle(GetOrderListQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetByUserIdAsync(query.UserId, cancellationToken, query.Pagination.Page, query.Pagination.PageSize);
    }
}
