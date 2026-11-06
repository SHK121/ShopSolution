using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.ValueObjects;
using Shop.Domain.Exceptions;

namespace Shop.Domain.Tests;

public class OrderTests
{
    private Order CreateOrder()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var order = Order.Create(userId);
        var item = OrderItem.Create(order.Id, productId, "newProduct", Money.Create(10000, Currency.KRW), 1);
        order.AddItem(item);
        return order;
    }

    [Fact]
    public void Order_Should_Be_Created_With_Draft_Status()
    {
        // Arrange and act
        var order = CreateOrder();

        // Assert
        Assert.Equal(OrderStatus.Draft, order.Status);
    }

    [Fact]
    public void Submit_Should_Throw_An_Exception_When_No_OrderItem_Was_Added()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var order = Order.Create(userId);

        // Act and assert
        Assert.Throws<ValidationException>(() => order.Submit());
    }

    [Fact]
    public void Submit_Should_Change_Order_Status()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.Submit();

        // Assert
        Assert.Equal(OrderStatus.Submitted, order.Status);
    }

    [Fact]
    public void Submitted_Order_Cannot_Be_Submitted_Again()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.Submit();

        // Assert
        Assert.Throws<ValidationException>(
            () => order.Submit());
    }

    [Fact]
    public void Submitted_Order_Should_Be_Paid()
    {
        // Arrange
        var order = CreateOrder();
        order.Submit();

        // Act
        order.Pay();

        // Assert
        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void Draft_Order_Cannot_Be_Paid()
    {
        // Arrange
        var order = CreateOrder();

        // Act and assert
        Assert.Throws<ValidationException>(
            () => order.Pay());
    }

    [Fact]
    public void Paid_Order_Should_Be_Shipped()
    {
        // Arrange
        var order = CreateOrder();

        order.Submit();
        order.Pay();

        // Act
        order.Ship();

        // Assert
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void Paid_Order_Cannot_Be_Cancelled()
    {
        // Arrange
        var order = CreateOrder();

        order.Submit();
        order.Pay();

        // Act and assert
        Assert.Throws<ValidationException>(
            () => order.Cancel());
    }

    [Fact]
    public void Draft_Order_Cannot_Be_Shipped()
    {
        var order = CreateOrder();

        Assert.Throws<ValidationException>(() => order.Ship());
    }

    [Fact]
    public void Submitted_Order_Cannot_Be_Shipped()
    {
        var order = CreateOrder();

        order.Submit();

        Assert.Throws<ValidationException>(() => order.Ship());
    }

    [Fact]
    public void Draft_Order_Should_Be_Cancelled()
    {
        var order = CreateOrder();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Submitted_Order_Should_Be_Cancelled()
    {
        var order = CreateOrder();

        order.Submit();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Paid_Order_Cannot_Be_Paid_Again()
    {
        // Arrange
        var order = CreateOrder();

        order.Submit();
        order.Pay();

        // Act and assert
        Assert.Throws<ValidationException>(
            () => order.Pay());
    }

    [Fact]
    public void Shipped_Order_Cannot_Be_Cancelled()
    {
        // Arrange
        var order = CreateOrder();

        order.Submit();
        order.Pay();
        order.Ship();

        // Act and assert
        Assert.Throws<ValidationException>(
            () => order.Cancel());
    }

    [Fact]
    public void Cancelled_Order_Cannot_Be_Submitted()
    {
        // Arrange
        var order = CreateOrder();

        order.Submit();
        order.Cancel();

        // Act and assert
        Assert.Throws<ValidationException>(
            () => order.Submit());
    }
}
