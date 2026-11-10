using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Api.Tests.Integration;
using Shop.Infrastructure.Persistence;

namespace Shop.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly ConcurrencySaveChangesInterceptor _concurrencyInterceptor = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json");
            config.AddJsonFile(
                path,
                optional: false,
                reloadOnChange: false);

            // Environment variables override JSON settings
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices((context, services) =>
        {
            var descriptor = services
                .SingleOrDefault(
                   d => d.ServiceType == typeof(DbContextOptions<ShopDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Test database connection string is missing.");
            }

            services.AddSingleton(_concurrencyInterceptor);
            services.AddDbContext<ShopDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(sp.GetRequiredService<ConcurrencySaveChangesInterceptor>());
            });

        });

    }

    public void InitializeDatabase()
    {
         using var scope = Services.CreateScope();
         var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
         dbContext.Database.Migrate();
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
        dbContext.Products.RemoveRange(dbContext.Products);
        dbContext.Orders.RemoveRange(dbContext.Orders);
        dbContext.Users.RemoveRange(dbContext.Users.Where(u => u.Username != "admin"));
        dbContext.SaveChanges();
    }

    public void EnableConcurrencyTest()
    {
        _concurrencyInterceptor.Reset();
        _concurrencyInterceptor.Enabled = true;
    }

    public void DisableConcurrencyTest()
    {
        _concurrencyInterceptor.Enabled = false;
    }
}
