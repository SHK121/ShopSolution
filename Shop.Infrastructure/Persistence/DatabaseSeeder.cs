using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Domain.Enums;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ShopDbContext context, IPasswordHasher passwordHasher)
    {
        var adminExists = await context.Users.AnyAsync(x => x.Role == UserRole.Admin);

        if (adminExists)
        {
            return;
        }

        var passwordHash = passwordHasher.Hash("admin123");

        var admin = User.Create("admin", passwordHash, UserRole.Admin);

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}
