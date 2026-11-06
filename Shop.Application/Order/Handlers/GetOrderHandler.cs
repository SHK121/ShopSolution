using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Order.Queries;

namespace Shop.Application.Order.Handlers;

public class GetOrderHandler
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Shop.Domain.Entities.Order> Handle(GetOrderQuery query, Guid userId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetAsync(query.OrderId, cancellationToken);

        if (order is null)
        {
            throw new NotFoundException("No order was found.");
        }

        if (order.UserId != userId)
        {
            throw new ForbiddenException("The user is not allowed to get the order.");
        }

        return order;
    }
}
