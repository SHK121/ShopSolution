using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.Exceptions;
using Shop.Domain.ValueObjects;

namespace Shop.Domain.Tests;

public class OrderItemTests
{
    [Fact]
    public void GetTotalPrice_Should_Return_UnitPrice_Multiplied_By_Quantity()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var orderItem = OrderItem.Create(orderId, productId, "Test Product", Money.Create(100, Currency.USD), 5);

        // Act
        var totalPrice = orderItem.GetTotalPrice();

        // Assert
        Assert.Equal(500, totalPrice.Amount);
        Assert.Equal(Currency.USD, totalPrice.Currency);
    }

    [Fact]
    public void Create_Should_Set_OrderItem_Information()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        var orderItem = OrderItem.Create(orderId, productId, "Test Product", Money.Create(100, Currency.USD), 5);

        // Assert
        Assert.Equal(orderId, orderItem.OrderId);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal("Test Product", orderItem.ProductName);
        Assert.Equal(100, orderItem.UnitPrice.Amount);
        Assert.Equal(Currency.USD, orderItem.UnitPrice.Currency);
        Assert.Equal(5, orderItem.Quantity);
    }

    [Fact]
    public void Create_Should_Throw_When_Quantity_Is_Zero()
    {
        Assert.Throws<ValidationException>(() =>
            OrderItem.Create(Guid.NewGuid(),
                Guid.NewGuid(), "Test Product", Money.Create(100, Currency.USD), 0));
    }

    [Fact]
    public void Create_Should_Throw_When_Quantity_Is_Negative()
    {
        Assert.Throws<ValidationException>(() =>
            OrderItem.Create(Guid.NewGuid(),
                Guid.NewGuid(), "Test Product", Money.Create(100, Currency.USD), -1));
    }
}
