using Moq;
using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Products.Handlers;
using Shop.Domain.Entities;
using Shop.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Tests.Products.Handlers;

public class DeleteProductHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_Product()
    {
        // Arrange
        var mockedRepo = new Mock<IProductRepository>();
        var product = Product.Create("Product 1", Money.Create(10, "USD"), 5);
        mockedRepo.Setup(x => x.GetAsync(product.Id, CancellationToken.None))
            .ReturnsAsync(product);
        var mockedUOW = new Mock<IUnitOfWork>();
        mockedUOW.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new DeleteProductHandler(mockedRepo.Object, mockedUOW.Object);


        // Act
        await handler.Handle(product.Id, CancellationToken.None);

        // Assert
        mockedRepo.Verify(x => x.GetAsync(product.Id, CancellationToken.None), Times.Once);
        mockedRepo.Verify(x => x.Remove(It.Is<Product>(p => p.Id == product.Id)), Times.Once);
        mockedUOW.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Product_Not_Found()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var mockedRepo = new Mock<IProductRepository>();
        mockedRepo.Setup(x => x.GetAsync(productId, CancellationToken.None)).ReturnsAsync((Product?)null);
        var mockedUOW = new Mock<IUnitOfWork>();
        var handler = new DeleteProductHandler(mockedRepo.Object, mockedUOW.Object);

        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(productId, CancellationToken.None));
        mockedRepo.Verify(x => x.GetAsync(productId, CancellationToken.None), Times.Once);
        mockedRepo.Verify(x => x.Remove(It.Is<Product>(p => p.Id == productId)), Times.Never);
        mockedUOW.Verify(x => x.SaveChangesAsync(), Times.Never);
        mockedRepo.VerifyNoOtherCalls();
        mockedUOW.VerifyNoOtherCalls();
    }
}
