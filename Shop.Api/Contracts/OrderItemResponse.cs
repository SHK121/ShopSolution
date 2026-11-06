using Shop.Domain.Enums;

namespace Shop.Api.Contracts;

public class OrderItemResponse
{
    // init은 객체를 생성할 때만 값을 설정할 수 있게 하는 것이야.
    public Guid ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public decimal UnitPriceAmount { get; init; }

    public Currency Currency { get; init; }

    public decimal TotalPrice { get; init; }

    public int Quantity { get; init; }
}
