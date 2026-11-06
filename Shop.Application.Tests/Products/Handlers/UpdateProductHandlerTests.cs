using Moq;
using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Products.Commands;
using Shop.Application.Products.Handlers;
using Shop.Domain.Entities;
using Shop.Domain.Enums;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Tests.Products.Handlers;

public class UpdateProductHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Product_When_Product_Exists()
    {
        // Arrange
        Product product = Product.Create("Product 1", Money.Create(20, "USD"), 5);
        var mockedRepo = new Mock<IProductRepository>();
        mockedRepo.Setup(r => r.GetAsync(product.Id, CancellationToken.None)).ReturnsAsync(product);
        var mockedUOW = new Mock<IUnitOfWork>();
        mockedUOW
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new UpdateProductHandler(mockedRepo.Object, mockedUOW.Object);
        var command = new UpdateProductCommand(
            product.Id,
            "Updated Product",
            30,
            "USD",
            10,
            "Updated Description"
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockedRepo.Verify(r => r.GetAsync(product.Id, CancellationToken.None), Times.Once);
        Assert.Equal("Updated Product", product.Name);
        Assert.Equal(30, product.Price.Amount);
        Assert.Equal(Currency.USD, product.Price.Currency);
        Assert.Equal(10, product.Stock);
        Assert.Equal("Updated Description", product.Description);
        mockedUOW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Product_Does_Not_Exist()
    {
        var productId = Guid.NewGuid();
        var mockedRepo = new Mock<IProductRepository>();
        mockedRepo.Setup(r => r.GetAsync(productId, CancellationToken.None)).ReturnsAsync((Product?)null);
        var mockedUOW = new Mock<IUnitOfWork>();
        var handler = new UpdateProductHandler(mockedRepo.Object, mockedUOW.Object);
        var command = new UpdateProductCommand(productId, "Updated Product", 30, "USD", 10, "Updated Description");


        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        mockedUOW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockedRepo.Verify(
            r => r.GetAsync(productId, CancellationToken.None),
            Times.Once);
        mockedRepo.VerifyNoOtherCalls();
        mockedUOW.VerifyNoOtherCalls();
    }
}
