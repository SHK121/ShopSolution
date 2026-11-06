using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.ValueObjects;
using Shop.Api.Mappers;
using OrderEntity = Shop.Domain.Entities.Order;

namespace Shop.Api.Tests.Mappers;

public class OrderMapperTests
{
    [Fact]
    public void ToResponse_Should_Map_Order_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var order = OrderEntity.Create(userId);

        var productId = Guid.NewGuid();
        var item = OrderItem.Create(
            order.Id,
            productId,
            "Test Product",
            Money.Create(100, Currency.USD),
            5);

        order.AddItem(item);

        // Act
        var response = OrderMapper.ToResponse(order);

        // Assert
        Assert.Equal(order.Id, response.Id);
        Assert.Equal(order.Status, response.Status);
        Assert.Single(response.Items);

        var responseItem = response.Items.First();

        Assert.Equal(productId, responseItem.ProductId);
        Assert.Equal("Test Product", responseItem.ProductName);
        Assert.Equal(100, responseItem.UnitPriceAmount);
        Assert.Equal(Currency.USD, responseItem.Currency);
        Assert.Equal(5, responseItem.Quantity);
        Assert.Equal(500, responseItem.TotalPrice);
    }
}
