using Xunit;
using Shop.Domain.Entities;
using Shop.Domain.ValueObjects;
using Shop.Domain.Enums;
using ValidationException = Shop.Domain.Exceptions.ValidationException;

namespace Shop.Domain.Tests;

public class ProductTests {
    private static Product CreateProduct() {
        return new Product("Keyboard", Money.Create(1000, Currency.KRW), 10);
    }

    [Fact]
    public void DecreaseStock_Should_Throw_When_Quantity_Is_Zero()
    {
        var product = CreateProduct();

        Assert.Throws<ValidationException>(() =>
        {
            product.DecreaseStock(0);
        });
    }

    [Fact]
    public void DecreaseStock_Should_Throw_When_Quantity_Is_Negative()
    {
        var product = CreateProduct();

        Assert.Throws<ValidationException>(() =>
        {
            product.DecreaseStock(-1);
        });
    }

    [Fact]
    public void IncreaseStock_Should_Throw_When_Quantity_Is_Zero()
    {
        var product = CreateProduct();

        Assert.Throws<ValidationException>(() =>
        {
            product.IncreaseStock(0);
        });
    }

    [Fact]
    public void Update_Should_Change_Product_Information()
    {
        var product = Product.Create(
            "Keyboard",
            Money.Create(100, Currency.USD),
            5);

        product.Update(
            "New Keyboard",
            Money.Create(200, Currency.USD),
            10,
            "Updated");

        Assert.Equal("New Keyboard", product.Name);
        Assert.Equal(200, product.Price.Amount);
        Assert.Equal(10, product.Stock);
        Assert.Equal("Updated", product.Description);
    }

    [Fact]
    public void CreateProduct_Should_Create_Product() {
        // arrange, act
        Product product = CreateProduct();

        // assert
        Assert.Equal("Keyboard", product.Name);
        Assert.Equal(10, product.Stock);
        Assert.Equal(1000, product.Price.Amount);
        Assert.Equal(Currency.KRW, product.Price.Currency);
    }

    [Fact]
    public void CreateProduct_Should_Throw_When_Name_Is_Empty() {
        // arrange, act, assert

        Assert.Throws<ValidationException>(() => {
            Product.Create(string.Empty, Money.Create(1000, "KRW"), 10);
        });
    }

    [Fact]
    public void CreateProduct_Should_Allow_Zero_Stock()
    {
        var product = new Product(
            "Keyboard",
            Money.Create(1000, Currency.KRW),
            0
        );

        Assert.Equal(0, product.Stock);
    }

    [Fact]
    public void IncreaseStock_Should_Increase_Stock() {
        // arrange
        Product product = CreateProduct();

        // act
        product.IncreaseStock(1);

        // assert
        Assert.Equal(11, product.Stock);
    }

    [Fact]
    public void DecreaseStock_Should_Throw_When_Stock_Is_Not_Enough() {
        // arrange
        Product product = CreateProduct();

        // act and assert
        Assert.Throws<ValidationException>(() => {
            product.DecreaseStock(12);
        });
    }

    [Fact]
    public void DecreaseStock_Should_Decrease_Stock() {
        // arrange
        Product product = CreateProduct();

        // act 
        product.DecreaseStock(3);

        // Assert
        Assert.Equal(7, product.Stock);
    }

    [Fact]
    public void ChangePrice_Should_Update_Price() {
        // arrange
        Money newPrice = Money.Create(500, Currency.KRW);
        Product product = CreateProduct();

        // act 
        product.ChangePrice(newPrice);

        // assert
        Assert.Equal(newPrice, product.Price);
        Assert.Equal(Currency.KRW, newPrice.Currency);
    }

    [Fact]
    public void IncreaseStock_Should_Throw_When_Quantity_Is_Negative()
    {
        Product product = CreateProduct();

        Assert.Throws<ValidationException>(() =>
        {
            product.IncreaseStock(-1);
        });
    }
}