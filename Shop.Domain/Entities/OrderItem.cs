using Shop.Domain.ValueObjects;
using Shop.Domain.Exceptions;

namespace Shop.Domain.Entities;

public class OrderItem {
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set;  }
    public int Quantity { get; private set; }

    private OrderItem()
    {
        // EF Core
    }

    private OrderItem(Guid orderId, Guid productId, string productName, Money unitPrice, int quantity) {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static OrderItem Create(Guid orderId, Guid productId, string productName, Money unitPrice, int quantity)
    {
        ValidateQuantity(quantity);
        return new OrderItem(orderId, productId, productName, unitPrice, quantity);
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Quantity must be bigger than 0.");
        }
    }

    public Money GetTotalPrice() {
        return Money.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);
    }
}