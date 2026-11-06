using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Auth.Commands;
using Shop.Application.Interfaces;
using Shop.Application.Products.Commands;
using Shop.Domain.Enums;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Shop.Api.Tests.Integration.Users;

public class AuthControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    private const string RegisterApi = "/api/auth/register";
    private const string CustomerUserName = "customerTest";
    private const string Password = "1234";

    public AuthControllerTests(CustomWebApplicationFactory factory) : base(factory) { }


    private CreateProductCommand CreateProductCommand()
    {
        return new CreateProductCommand("Test Product", 100, "USD", 100, "Test Product");
    }

    [Fact]
    public async Task Register_Should_Create_New_User()
    {
        // Arrange
        string username = $"customer_{Guid.NewGuid()}";
        const string password = Password;
        var registerCommand = new RegisterCommand(username, password);

        // Act
        var createResponse = await Client.PostAsJsonAsync(RegisterApi, registerCommand);
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var user = await repository.GetByUsernameAsync(username);

        // Assert
        createResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(user);
        Assert.Equal(username, user.Username);
        Assert.Equal(UserRole.Customer, user.Role);
        Assert.NotEqual(password, user.PasswordHash);
    }

    [Fact]
    public async Task Login_Should_Return_Token()
    {
        // Act and assert
        var token = await CreateAdminTokenAsync();
        Assert.NotNull(token);
    }

    [Fact]
    public async Task Admin_Should_Access_Admin_Endpoint()
    {
        // Arrange
        var request = CreateAuthenticatedRequest(HttpMethod.Post, ProductsApi, AdminToken!);

        // Act
        request.Content = JsonContent.Create(CreateProductCommand());
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Customer_Should_Receive_Forbidden()
    {
        // Arrange 
        using var request = new HttpRequestMessage(HttpMethod.Post, ProductsApi);

        var tokenAsCustomer = await CreateCustomerTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAsCustomer);

        // Act
        request.Content = JsonContent.Create(CreateProductCommand());
        var response = await Client.SendAsync(request);

        // Assert
        // ASP.NET Core throws an error
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Login_Should_Reject_Invalid_Password()
    {
        // Arrange
        var registerCommand = new RegisterCommand(CustomerUserName, Password);

        var registerResponse =
            await Client.PostAsJsonAsync(
                RegisterApi,
                registerCommand);

        registerResponse.EnsureSuccessStatusCode();
        var command = new LoginCommand(CustomerUserName, "12345");

        // Act and assert
        var response = await Client.PostAsJsonAsync(LoginApi, command);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Unauthenticated_User_Should_Receive_Unauthorized()
    {
        // Act (not logged in)
        var response = await Client.PostAsJsonAsync(
            ProductsApi,
            CreateProductCommand());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
