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

public class PayOrderHandlerTests
{
    [Fact]
    public async Task Pay_Should_Change_Order_Status()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockedRepo = new Mock<IOrderRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();

        var handler = new PayOrderHandler(mockedRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        var orderItem = OrderItem.Create(order.Id, Guid.NewGuid(), "product", Money.Create(5, Currency.USD), 5);
        order.AddItem(orderItem);
        Assert.Equal(OrderStatus.Draft, order.Status);
        order.Submit();
        var command = new PayOrderCommand(order.Id);
        mockedRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        // Act
        await handler.Handle(command, userId, CancellationToken.None);

        // Assert
        Assert.Equal(OrderStatus.Paid, order.Status);
        mockedUOW.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Pay_Should_Throw_An_Exception_For_Draft_Order()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockedRepo = new Mock<IOrderRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();

        var handler = new PayOrderHandler(mockedRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        var orderItem = OrderItem.Create(order.Id, Guid.NewGuid(), "product", Money.Create(5, Currency.USD), 5);
        order.AddItem(orderItem);
        Assert.Equal(OrderStatus.Draft, order.Status);
        var command = new PayOrderCommand(order.Id);
        mockedRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        // Act and assert
        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, userId, CancellationToken.None));
        mockedUOW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Pay_Should_Throw_An_Exception_When_No_Order_Was_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockedRepo = new Mock<IOrderRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();

        var handler = new PayOrderHandler(mockedRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        mockedRepo
        .Setup(x => x.GetAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync((OrderEntity?)null);
        var command = new PayOrderCommand(order.Id);

        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, userId, CancellationToken.None));
        mockedUOW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Pay_Should_Throw_An_Exception_For_Forbidden_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var mockedRepo = new Mock<IOrderRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();

        var handler = new PayOrderHandler(mockedRepo.Object, mockedUOW.Object);
        var order = OrderEntity.Create(userId);
        var orderItem = OrderItem.Create(order.Id, Guid.NewGuid(), "product", Money.Create(5, Currency.USD), 5);
        order.AddItem(orderItem);
        Assert.Equal(OrderStatus.Draft, order.Status);
        var command = new PayOrderCommand(order.Id);
        mockedRepo.Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        // Act and assert
        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, otherUserId, CancellationToken.None));
        mockedUOW.Verify(x => x.SaveChangesAsync(), Times.Never);
        Assert.Equal(OrderStatus.Draft, order.Status);
    }
}
