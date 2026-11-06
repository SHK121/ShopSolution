using Shop.Domain.Enums;
using Shop.Domain.Exceptions;

namespace Shop.Domain.ValueObjects;

public class Money {
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency) {
        ValidateAmount(amount);

        Amount = amount;
        Currency = currency;
    }

    private Money()
    {
        // EF Core 용
    }

    public static Money Create(decimal amount, string currency)
    {
        ValidateAmount(amount);
        var parsedCurrency = ValidateCurrency(currency);

        return new Money(amount, parsedCurrency);
    }

    public static Money Create(decimal amount, Currency currency)
    {
        ValidateAmount(amount);

        return new Money(amount, currency);
    }

    private static void ValidateAmount(decimal amount) {
        if (amount < 0) {
            throw new ValidationException("The amount must be positive.");
        }
    }

    private static Currency ValidateCurrency(string currency)
    {
        if(!Enum.TryParse<Currency>(currency, out var parsedCurrency))
        {
            throw new ValidationException("Invalid currency.");
        }
        return parsedCurrency;
    }
}