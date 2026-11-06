using Shop.Domain.Enums;
using Shop.Domain.ValueObjects;
using Shop.Domain.Exceptions;

namespace Shop.Domain.Entities;

public class Product {

// Sell Business logic
// Decrease Behavior

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public int Stock { get; private set; }
    public string Description { get; private set; }

    public byte[] RowVersion { get; private set; } = null; // Concurrency token for EF Core, SQL Server updates the value automatically.    

    public void ChangePrice(Money price) {
        Price = price;
    }

    private static void ValidateStock(int stock)
    {
        if (stock < 0)
            throw new ValidationException("The stock cannot be negative.");
    }

    private static void ValidateDescription(string description)
    {
        if (description.Length > 500)
            throw new ValidationException("The description cannot exceed 500 characters.");
    }

    public void IncreaseStock(int quantity) {
        if (quantity <= 0)
        {
            throw new ValidationException(
                "Quantity must be bigger than 0.");
        }

        Stock += quantity;
    }

    public void DecreaseStock(int quantity) {
        if (quantity <= 0)
        {
            throw new ValidationException(
                "Quantity must be bigger than 0.");
        }

        if (Stock < quantity)
        {
            throw new ValidationException(
                "Not enough stock.");
        }

        Stock -= quantity;
    }

    private Product()
    {
        // EF Core 용
    }

    public Product(string name, Money price, int stock, string description = "") {

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
        Description = description;
    }

    public static Product Create(string name, Money money, int stock, string description = "")
    {
        ValidateName(name);
        ValidateStock(stock);
        ValidateDescription(description);

        return new Product(name, money, stock, description);
    }

    public void Update(
        string name,
        Money price,
        int stock,
        string description)
    {
        ValidateName(name);
        ValidateStock(stock);
        ValidateDescription(description);

        Name = name;
        Price = price;
        Stock = stock;
        Description = description;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("The name cannot be empty.");
        }
    }

    public void ChangeDescription(string description) {
        ValidateDescription(description);
        Description = description;
    }
}