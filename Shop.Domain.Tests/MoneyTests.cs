using Shop.Domain.Enums;
using Shop.Domain.ValueObjects;
using ValidationException = Shop.Domain.Exceptions.ValidationException;

namespace Shop.Domain.Tests;

public class MoneyTests {
    [Fact]
    public void Money_Should_Throw_When_Amount_Is_Negative() {
        // arrange, act, assert
        Assert.Throws<ValidationException>(() => {
            Money.Create(-1000, Currency.KRW);
        });
    }

    [Fact]
    public void Money_Should_Create_With_Valid_Amount()
    {
        var money = Money.Create(1000, Currency.KRW);

        Assert.Equal(1000, money.Amount);
        Assert.Equal(Currency.KRW, money.Currency);
    }
}