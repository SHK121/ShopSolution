using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Auth.Handlers;
using Shop.Application.Order.Handlers;
using Shop.Application.Products.Handlers;

namespace Shop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<UpdateProductHandler>();
        services.AddScoped<DeleteProductHandler>();
        services.AddScoped<GetProductHandler>();
        services.AddScoped<GetProductListHandler>();
        services.AddScoped<GetProductsByIdsHandler>();
        services.AddScoped<RegisterHandler>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<SubmitOrderHandler>();
        services.AddScoped<PayOrderHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<GetOrderHandler>();
        services.AddScoped<GetOrderListHandler>();
        services.AddScoped<CancelOrderHandler>();
        services.AddScoped<ShipOrderHandler>();

        // Register application services here
        return services;
    }
}
