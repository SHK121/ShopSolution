using Shop.Api.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Shop.Api.Tests.Integration.Products;

public class ProductControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public ProductControllerTests(CustomWebApplicationFactory factory) : base(factory) {  }

    [Fact]
    public async Task UpdateProduct_Should_Update_Product()
    {
        // Arrange
        var createdProductId = await CreateProductAsync(CreateProductCommand());
        var url = $"/api/products/{createdProductId}";
        // Only admin user can update a product.
        var httpRequest = CreateAuthenticatedRequest(HttpMethod.Put, url, AdminToken!);
        var updateRequest = new UpdateProductRequest("Updated Product", 200, "USD", 200, "Updated Product");
        httpRequest.Content = JsonContent.Create(updateRequest);

        // Act
        var response = await Client.SendAsync(httpRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updated = await Client.GetAsync(url);
        var updatedProduct = await updated.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(updatedProduct);
        Assert.Equal(updateRequest.Name, updatedProduct.Name);
        Assert.Equal(updateRequest.Price, updatedProduct.Price);
        Assert.Equal(updateRequest.Currency, updatedProduct.Currency);
        Assert.Equal(updateRequest.Stock, updatedProduct.Stock);
        Assert.Equal(updateRequest.Description, updatedProduct.Description);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return_Forbidden_For_Non_Admin_User()
    {
        // Arrange
        var createdProductId = await CreateProductAsync(CreateProductCommand());
        var url = $"/api/products/{createdProductId}";

        // Non-admin user token
        var customerToken = await CreateCustomerTokenAsync();
        var httpRequest = CreateAuthenticatedRequest(HttpMethod.Put, url, customerToken!);
        var updateRequest = new UpdateProductRequest("Updated Product", 200, "USD", 200, "Updated Product");
        httpRequest.Content = JsonContent.Create(updateRequest);

        // Act
        var response = await Client.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_Return_Forbidden_For_Non_Admin_User()
    {
        // Arrange
        var createdProductId = await CreateProductAsync(CreateProductCommand());
        var url = $"/api/products/{createdProductId}";

        // Non-admin user token
        var customerToken = await CreateCustomerTokenAsync();
        var httpRequest = CreateAuthenticatedRequest(HttpMethod.Delete, url, customerToken!);

        // Act
        var response = await Client.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_Delete_Product()
    {
        // Arrange
        var createdProductId = await CreateProductAsync(CreateProductCommand());

        // Act
        var request = CreateAuthenticatedRequest(HttpMethod.Delete, $"/api/products/{createdProductId}", AdminToken!);
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var getResponse = await Client.GetAsync($"/api/products/{createdProductId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetProducts_Should_Return_Created_Product()
    {
        // Arrange
        var command = CreateProductCommand();
        var createdProductId = await CreateProductAsync(command);

        // Act
        var response =
            await Client.GetAsync($"/api/products/{createdProductId}");

        // Assert
        response.EnsureSuccessStatusCode();

        var product =
            await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);
        Assert.Equal(command.Name, product.Name);
        Assert.Equal(command.Price, product.Price);
        Assert.Equal(command.Currency, product.Currency);
        Assert.Equal(command.Stock, product.Stock);
    }

    [Fact]
    public async Task GetProducts_Should_Return_Correct_Pagination_Metadata()
    {
        // Arrange
        await CreateProductAsync(CreateProductCommand());

        var response = await Client.GetAsync(
            "/api/products?page=1&pageSize=20");

        // Act
        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<
                PaginatedResponse<ProductResponse>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }
}
