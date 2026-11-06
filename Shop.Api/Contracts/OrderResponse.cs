using Shop.Domain.Enums;

namespace Shop.Api.Contracts;

public class OrderResponse
{
    public Guid Id { get; init; }
    public OrderStatus Status { get; init; }
    public Currency? OrderCurrency { get; init; }

    public DateTime CreatedAt { get; init; }
    public List<OrderItemResponse> Items { get; init; } = [];
}
