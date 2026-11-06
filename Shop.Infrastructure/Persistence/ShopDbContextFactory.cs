using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shop.Infrastructure.Persistence;

public class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
{
    public ShopDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<ShopDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=ShopDb;User Id=sa;Password=Shop123!;TrustServerCertificate=True;Encrypt=False;"
        );

        return new ShopDbContext(optionsBuilder.Options);
    }
}
