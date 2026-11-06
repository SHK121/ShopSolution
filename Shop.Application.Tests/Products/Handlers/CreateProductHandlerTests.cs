using Xunit;
using Moq;
using Shop.Application.Interfaces;
using Shop.Application.Products.Commands;
using Shop.Application.Products.Handlers;
using Shop.Domain.Entities;

namespace Shop.Application.Tests.Products.Handlers;

public class CreateProductHandlerTests {
    private CreateProductCommand CreateCommand() {
        return new CreateProductCommand(
            "Keyboard",
            10000,
            "KRW",
            10,
            "Good price");
    }

    [Fact]
    public async Task Handle_Should_Create_Product_And_Return_Product_Id()
    {
        // Arrange
        Product? savedProduct = null;

        var mockedRepo = new Mock<IProductRepository>();
        var mockedUnitOfWork = new Mock<IUnitOfWork>();

        mockedRepo
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => savedProduct = p)
            .Returns(Task.CompletedTask);
        
        mockedUnitOfWork
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var handler = new CreateProductHandler(mockedRepo.Object, mockedUnitOfWork.Object);
        var command = CreateCommand();


        // Act
        var result = await handler.Handle(command, new CancellationToken());

        // Assert
        Assert.NotNull(savedProduct);
        Assert.Equal(savedProduct!.Id, result);
        mockedRepo.Verify(
            x => x.AddAsync(It.IsAny<Product>()),
            Times.Once);
    }
}