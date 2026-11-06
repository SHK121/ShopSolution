using Shop.Api.Contracts;
using OrderEntity = Shop.Domain.Entities.Order;
using Shop.Domain.Entities;

namespace Shop.Api.Mappers;

public static class OrderMapper
{
    public static OrderResponse ToResponse(OrderEntity order)
    {
        // Entity → 생성자/Factory로 불변식 보장, DTO → init + object initializer로 데이터 전달.
        return new OrderResponse
        {
            Id = order.Id,
            Status = order.Status,
            OrderCurrency = order.OrderCurrency,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => ToResponse(i)).ToList()
        };
    }

    public static OrderItemResponse ToResponse(OrderItem orderItem)
    {
        return new OrderItemResponse {
            ProductId = orderItem.ProductId,
            ProductName = orderItem.ProductName,
            UnitPriceAmount = orderItem.UnitPrice.Amount,
            Currency = orderItem.UnitPrice.Currency,
            TotalPrice = orderItem.GetTotalPrice().Amount,
            Quantity = orderItem.Quantity
        };
    }
}
