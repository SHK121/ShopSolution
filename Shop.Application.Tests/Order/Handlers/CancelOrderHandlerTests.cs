using Moq;
using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Order.Commands;
using Shop.Application.Order.Handlers;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.Exceptions;
using Shop.Domain.ValueObjects;
using OrderEntity = Shop.Domain.Entities.Order;

namespace Shop.Application.Tests.Order.Handlers;

public class CancelOrderHandlerTests
{
    [Fact]
    public async Task Cancel_Should_Change_Order_Status()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockedOrderRepo = new Mock<IOrderRepository>();
        var mockedProductRepo = new Mock<IProductRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();

        var handler = new CancelOrderHandler(mockedOrderRepo.Object, mockedProductRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        var orderItem = OrderItem.Create(order.Id, Guid.NewGuid(), "product", Money.Create(5, Currency.USD), 5);
        order.AddItem(orderItem);
        Assert.Equal(OrderStatus.Draft, order.Status);
        var command = new CancelOrderCommand(order.Id);
        mockedOrderRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        // Act
        await handler.Handle(command, userId, CancellationToken.None);

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        mockedUOW.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Cancel_Should_Throw_An_Exception_When_Order_Was_Paid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockedOrderRepo = new Mock<IOrderRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();
        var mockedProductRepo = new Mock<IProductRepository>();

        var handler = new CancelOrderHandler(mockedOrderRepo.Object, mockedProductRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        var orderItem = OrderItem.Create(order.Id, Guid.NewGuid(), "product", Money.Create(5, Currency.USD), 5);
        order.AddItem(orderItem);
        Assert.Equal(OrderStatus.Draft, order.Status);
        order.Submit();
        order.Pay();
        var command = new CancelOrderCommand(order.Id);
        mockedOrderRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        // Act and assert
        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, userId, CancellationToken.None));
    }

    [Fact]
    public async Task Cancel_Should_Throw_An_Exception_When_No_Order_Was_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockedOrderRepo = new Mock<IOrderRepository>();
        var mockedProductRepo = new Mock<IProductRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();

        var handler = new CancelOrderHandler(mockedOrderRepo.Object, mockedProductRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        mockedOrderRepo
        .Setup(x => x.GetAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync((OrderEntity?)null);
        var command = new CancelOrderCommand(order.Id);

        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, userId, CancellationToken.None));
    }

    [Fact]
    public async Task Cancel_Should_Throw_An_Exception_For_Forbidden_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var mockedOrderRepo = new Mock<IOrderRepository>();
        var mockedProductRepo = new Mock<IProductRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();

        var handler = new CancelOrderHandler(mockedOrderRepo.Object, mockedProductRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        var orderItem = OrderItem.Create(order.Id, Guid.NewGuid(), "product", Money.Create(5, Currency.USD), 5);
        order.AddItem(orderItem);
        Assert.Equal(OrderStatus.Draft, order.Status);
        var command = new CancelOrderCommand(order.Id);
        mockedOrderRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        // Act and assert
        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, otherUserId, CancellationToken.None));
        Assert.Equal(OrderStatus.Draft, order.Status);
    }
}
