using OrderEntity = Shop.Domain.Entities.Order;
using Shop.Application.Common.Pagination;

namespace Shop.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(OrderEntity order, CancellationToken cancellationToken);
    Task<OrderEntity?> GetAsync(Guid orderId, CancellationToken cancellationToken);

    Task<PaginatedResult<OrderEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page, int pageSize);
}
