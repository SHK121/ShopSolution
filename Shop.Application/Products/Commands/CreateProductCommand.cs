
namespace Shop.Application.Products.Commands;

public record CreateProductCommand
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "KRW";
    public int Stock { get; init; }
    public string Description { get; init; } = string.Empty;

    public CreateProductCommand(string name, decimal price, string currency, int stock, string description = "")
    {
        Name = name;
        Price = price;
        Currency = currency;
        Stock = stock;
        Description = description;
    }
}

