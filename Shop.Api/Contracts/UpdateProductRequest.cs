namespace Shop.Api.Contracts;

public record UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Currency { get; set; } = string.Empty;

    public int Stock { get; set; }

    public string Description { get; set; } = string.Empty;

    public UpdateProductRequest(string name, decimal price, string currency, int stock, string description)
    {
        Name = name;
        Price = price;
        Currency = currency;
        Stock = stock;
        Description = description;
    }
}
