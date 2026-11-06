using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Shop.Application.Auth.Commands;
using Shop.Application.Products.Commands;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shop.Domain.Entities;
using Shop.Application.Order.Commands;
using Shop.Application.Interfaces;

namespace Shop.Api.Tests.Integration;

public class IntegrationTestBase : IAsyncLifetime
{
    protected const string Key_Token = "token";
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;
    protected const string LoginApi = "/api/auth/login";
    protected const string AdminUserName = "admin";
    protected const string ProductsApi = "/api/products";
    private const string OrderApi = "/api/orders";

    protected string? AdminToken { get; private set; }

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    // Authentication
    protected async Task<string?> CreateAdminTokenAsync()
    {
        var loginCommand = new LoginCommand(AdminUserName, "admin123");
        var response = await Client.PostAsJsonAsync(LoginApi, loginCommand);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        return JObject.Parse(responseBody).Value<string>(Key_Token);
    }

    protected async Task<string?> CreateCustomerTokenAsync()
    {
        var username = $"customer_{Guid.NewGuid()}";
        var password = "1234";

        var registerCommand = new RegisterCommand(username, password);
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerCommand);
        registerResponse.EnsureSuccessStatusCode();

        var loginCommand = new LoginCommand(username, password);
        var loginResponse = await Client.PostAsJsonAsync(LoginApi, loginCommand);
        loginResponse.EnsureSuccessStatusCode();
        var responseBody = await loginResponse.Content.ReadAsStringAsync();

        return JObject.Parse(responseBody).Value<string>(Key_Token);
    }

    // Http
    protected HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method,
        string url,
        string token)
    {
        var request = new HttpRequestMessage(method, url);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return request;
    }

    // Entities

    protected async Task<Order> CreateOrderAsync(Guid productId, string userToken, int quantity = 5, CancellationToken cancellationToken = default)
    {
        // var userToken = await LoginAsAdminAsync();
        var request = CreateAuthenticatedRequest(HttpMethod.Post, OrderApi, userToken!);
        var orderCommand = new CreateOrderCommand(new List<CreateOrderItemCommand>
        {
            new CreateOrderItemCommand(productId, quantity)
        });
        request.Content = JsonContent.Create(orderCommand);
        var response = await Client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var orderId = await response.Content.ReadFromJsonAsync<Guid>();

        using var scope = Factory.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var createdOrder = await repository.GetAsync(orderId, cancellationToken);

        return createdOrder!;
    }

    protected async Task<Guid> CreateProductAsync(CreateProductCommand command)
    {
        // Only admin user can create a new product.
        var request = CreateAuthenticatedRequest(HttpMethod.Post, ProductsApi, AdminToken!);
        request.Content = JsonContent.Create(command);
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    protected CreateProductCommand CreateProductCommand() => new CreateProductCommand("Test Product", 100, "USD", 100, "Test Product");

    // DB Lifecycle
    public async Task InitializeAsync()
    {
        //이 구조라면 이 클래스 전체가 시작될 때 한 번 Initialize되고, 클래스가 끝날 때 한 번 Dispose돼.
        Factory.InitializeDatabase();

        AdminToken = await CreateAdminTokenAsync();
    }

    public Task DisposeAsync()
    {
        Factory.ResetDatabase();
        return Task.CompletedTask;
    }

}
