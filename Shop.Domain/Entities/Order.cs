using Shop.Domain.Enums;
using Shop.Domain.ValueObjects;
using Shop.Domain.Exceptions;

namespace Shop.Domain.Entities;

public class Order {
    public Guid Id { get; }
    public Guid UserId { get; }
    public OrderStatus Status { get; private set; }
    public Currency? OrderCurrency { get; private set; }

    private readonly List<OrderItem> _items = new();

    public IReadOnlyCollection<OrderItem> Items => _items;

    public DateTime CreatedAt { get; private set; }

    private Order()
    {
        // EF Core
    }

    private Order(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Draft;
    }

    private bool IsDraft()
    {
        return Status == OrderStatus.Draft;
    }

    public static Order Create(Guid userId)
    {
        return new Order(userId);
    }

    public void RemoveItem(OrderItem item)
    {
        if (!IsDraft())
        {
            throw new ValidationException(
                "Items can only be removed from a draft order.");
        }

        if (!_items.Remove(item))
            throw new ValidationException(
                "The order item does not exist in this order.");
    }

    public void AddItem(OrderItem item)
    {
        if (!IsDraft())
        {
            throw new ValidationException(
                "Items can only be added to a draft order.");
        }

        if (!_items.Any())
        {
            OrderCurrency = item.UnitPrice.Currency;
        }
        else
        {
            if (OrderCurrency is null)
            {
                throw new ValidationException(
                    "Order currency is not set.");
            }

            if (item.UnitPrice.Currency != OrderCurrency)
            {
                throw new ValidationException(
                    "The price currency does not match the order currency.");
            }
        }
        _items.Add(item);
    }

    public Money GetTotalPrice()
    {
        if (!OrderCurrency.HasValue)
        {
            throw new ValidationException("Order currency is not set.");
        }

        var total = _items.Sum(i => i.GetTotalPrice().Amount);
        return Money.Create(total, OrderCurrency.Value);
    }

    public void Submit()
    {
        if (!IsDraft())
        {
            throw new ValidationException(
                "Only a draft order can be submitted.");
        }

        if (!_items.Any())
        {
            throw new ValidationException(
                "An order must contain at least one item.");
        }

        if (OrderCurrency is null)
        {
            throw new ValidationException(
                "Order currency is not set.");
        }

        Status = OrderStatus.Submitted;
    }

    public void Pay() {
        if (Status != OrderStatus.Submitted) {
            throw new ValidationException("Only submitted order can be paid.");
        }

        Status = OrderStatus.Paid;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
        {
            throw new ValidationException("Only paid order can be shipped.");
        }

        Status = OrderStatus.Shipped;
    }

    public void Cancel() {
        if (Status != OrderStatus.Draft &&
            Status != OrderStatus.Submitted)
        {
            throw new ValidationException(
                "Only draft or submitted orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}