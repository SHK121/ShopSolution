using Moq;
using Shop.Application.Interfaces;
using Shop.Application.Order.Queries;
using Shop.Application.Order.Handlers;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.ValueObjects;
using OrderEntity = Shop.Domain.Entities.Order;
using Shop.Application.Common.Pagination;

namespace Shop.Application.Tests.Order.Handlers;

public class GetOrderListHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Paginated_Result()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockedRepo = new Mock<IOrderRepository>();
        var handler = new GetOrderListHandler(mockedRepo.Object);

        var order = OrderEntity.Create(userId);
        var orderItem = OrderItem.Create(
            order.Id,
            Guid.NewGuid(),
            "product",
            Money.Create(5, Currency.USD),
            5);

        order.AddItem(orderItem);

        var expectedResult = new PaginatedResult<OrderEntity>(
            new List<OrderEntity> { order },
            1,
            1,
            1);

        mockedRepo
            .Setup(x => x.GetByUserIdAsync(
                userId,
                It.IsAny<CancellationToken>(),
                1,
                1))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(
            new GetOrderListQuery(userId, new PageRequest(1, 1)),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(order.Id, result.Items[0].Id);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(1, result.TotalCount);

        mockedRepo.Verify(x => x.GetByUserIdAsync(
            userId,
            It.IsAny<CancellationToken>(),
            1,
            1), Times.Once);
    }
}
