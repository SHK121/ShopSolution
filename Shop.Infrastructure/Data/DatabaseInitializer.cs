using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(ShopDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        await DatabaseSeeder.SeedAsync(context, passwordHasher);
    }
}
