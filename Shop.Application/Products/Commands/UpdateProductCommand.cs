using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Products.Commands;

public record UpdateProductCommand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Currency { get; set; } = string.Empty;

    public int Stock { get; set; }

    public string Description { get; set; } = string.Empty;

    public UpdateProductCommand(Guid id, string name, decimal price, string currency, int stock, string description = "")
    {
        Id = id;
        Name = name;
        Price = price;
        Currency = currency;
        Stock = stock;
        Description = description;
    }
}
