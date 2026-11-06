using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Interfaces;
using System.Net.Http.Json;
using Shop.Application.Common.Pagination;
using Shop.Api.Contracts;
using System.Net;

namespace Shop.Api.Tests.Integration.Orders;

public class OrderControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{

    public OrderControllerTests(CustomWebApplicationFactory factory) : base(factory) { }    

    [Fact]
    public async Task SubmitOrder_Should_Change_OrderStatus_And_Decrease_Stock()
    {
        // Arrange
        var userToken = await CreateAdminTokenAsync();

        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);

        var createdOrder = await CreateOrderAsync(productId, userToken!);
        Assert.Equal(OrderStatus.Draft, createdOrder.Status);

        var request = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/orders/{createdOrder.Id}/submit", userToken!);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();

        using var scope = Factory.Services.CreateScope();

        var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var updatedOrder = await orderRepo.GetAsync(createdOrder.Id, CancellationToken.None);
        var updatedProduct = await productRepo.GetAsync(productId, CancellationToken.None);

        Assert.NotNull(updatedOrder);
        Assert.NotNull(updatedProduct);
        Assert.Equal(OrderStatus.Submitted, updatedOrder.Status);
        Assert.Equal(createProductCommand.Stock - 5, updatedProduct.Stock);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }


    [Fact]
    public async Task CancelOrder_Should_Change_OrderStatus_And_Restore_Stock()
    {
        // Arrange
        var userToken = await CreateAdminTokenAsync();
        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);
        var createdOrder = await CreateOrderAsync(productId, userToken!);

        Assert.Equal(OrderStatus.Draft, createdOrder.Status);

        // Submit
        var submitRequest = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/orders/{createdOrder.Id}/submit", userToken!);
        var submitResponse = await Client.SendAsync(submitRequest);
        submitResponse.EnsureSuccessStatusCode();

        // Verify submit result with a fresh scope
        using var submitScope = Factory.Services.CreateScope();

        var submitOrderRepo = submitScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var submitProductRepo = submitScope.ServiceProvider.GetRequiredService<IProductRepository>();
        var submittedOrder = await submitOrderRepo.GetAsync(createdOrder.Id, CancellationToken.None);
        var submittedProduct = await submitProductRepo.GetAsync(productId, CancellationToken.None);

        Assert.NotNull(submittedOrder);
        Assert.NotNull(submittedProduct);

        Assert.Equal(OrderStatus.Submitted, submittedOrder.Status);
        var stockAfterSubmit = submittedProduct.Stock;

        // Cancel
        var cancelRequest = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/orders/{createdOrder.Id}/cancel", userToken!);

        // Act
        var cancelResponse = await Client.SendAsync(cancelRequest);

        // Assert
        cancelResponse.EnsureSuccessStatusCode();

        // Verify cancellation result with a fresh scope
        using var verifyScope = Factory.Services.CreateScope();
        var verifyOrderRepo = verifyScope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var verifyProductRepo = verifyScope.ServiceProvider.GetRequiredService<IProductRepository>();

        var cancelledOrder = await verifyOrderRepo.GetAsync(createdOrder.Id, CancellationToken.None);
        var restoredProduct = await verifyProductRepo.GetAsync(productId, CancellationToken.None);

        Assert.NotNull(cancelledOrder);
        Assert.NotNull(restoredProduct);
        Assert.Equal(OrderStatus.Cancelled, cancelledOrder.Status);
        Assert.Equal(stockAfterSubmit + 5, restoredProduct.Stock);
        Assert.Equal(HttpStatusCode.NoContent, cancelResponse.StatusCode);
    }


    [Fact]
    public async Task CreateOrder_Should_Create_New_Order()
    {
        // Arrange and act
        var userToken = await CreateAdminTokenAsync();
        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);
        var createdOrder = await CreateOrderAsync(productId, userToken!);

        // Assert
        Assert.NotNull(createdOrder);
        Assert.Single(createdOrder.Items);

        var createdItem = createdOrder.Items.First();

        Assert.Equal(productId, createdItem.ProductId);
        Assert.Equal(5, createdItem.Quantity);
        Assert.Equal(createProductCommand.Currency, createdOrder.OrderCurrency!.ToString());
        Assert.Equal(createProductCommand.Price * 5, createdOrder.GetTotalPrice().Amount);
        Assert.Equal(OrderStatus.Draft, createdOrder.Status);
    }

    [Fact]
    public async Task Customer_Cannot_Submit_Admin_Order()
    {
        // Arrange
        var adminToken = await CreateAdminTokenAsync();
        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);
        var createdOrder = await CreateOrderAsync(productId, adminToken!);

        var customerToken = await CreateCustomerTokenAsync();

        var url = $"/api/orders/{createdOrder.Id}/submit";
        var request = CreateAuthenticatedRequest(HttpMethod.Post, url, customerToken!);


        // Act and assert
        var response = await Client.SendAsync(request);
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetOrder_Should_Return_Order()
    {
        // Arrange
        var userToken = await CreateAdminTokenAsync();
        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);
        var createdOrder = await CreateOrderAsync(productId, userToken!);

        var url = $"/api/orders/{createdOrder.Id}";
        var request = CreateAuthenticatedRequest(HttpMethod.Get, url, userToken!);

        // Act
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var returnedOrder = await response.Content.ReadFromJsonAsync<OrderResponse>();

        // Assert
        Assert.Equal(createdOrder.Id, returnedOrder?.Id);
        Assert.Equal(createdOrder.Status, returnedOrder?.Status);
        Assert.Equal(createdOrder.Items.Count, returnedOrder?.Items.Count);
        Assert.Equal(createdOrder.CreatedAt, returnedOrder?.CreatedAt);
        Assert.Equal(createdOrder.OrderCurrency, returnedOrder?.OrderCurrency);
    }

    [Fact]
    public async Task GetOrder_Should_Return_NotFound_When_Order_Does_Not_Exist()
    {
        // Arrange
        var userToken = await CreateAdminTokenAsync();
        var orderId = Guid.NewGuid();

        var url = $"/api/orders/{orderId}";
        var request = CreateAuthenticatedRequest(HttpMethod.Get, url, userToken!);

        // Act 
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrder_Should_Return_Forbidden_For_Different_User()
    {
        // Arrange
        var userToken = await CreateAdminTokenAsync();
        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);
        var createdOrder = await CreateOrderAsync(productId, userToken!);
        var anotherUserToken = await CreateCustomerTokenAsync();

        var url = $"/api/orders/{createdOrder.Id}";
        var request = CreateAuthenticatedRequest(HttpMethod.Get, url, anotherUserToken!);

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetOrderList_Should_Return_Order_List()
    {
        // Arrange
        var currentUserToken = await CreateCustomerTokenAsync();
        var secondUserToken = await CreateAdminTokenAsync();
        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);
        var firstOrder = await CreateOrderAsync(productId, currentUserToken!);
        var secondOrder = await CreateOrderAsync(productId, currentUserToken!);
        var orderByAnotherUser = await CreateOrderAsync(productId, secondUserToken!);

        var url = $"/api/orders";
        var request = CreateAuthenticatedRequest(HttpMethod.Get, url, currentUserToken!);

        // Act
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<OrderResponse>>();
        var orders = result?.Items;

        // Assert
        Assert.NotNull(orders);
        Assert.Equal(2, orders.Count);

        Assert.Contains(orders, x => x.Id == firstOrder.Id);
        Assert.Contains(orders, x => x.Id == secondOrder.Id);
        Assert.DoesNotContain(orders, x => x.Id == orderByAnotherUser.Id);
    }

    public static IEnumerable<object[]> GetPaginatedTestData()
    {
        yield return new object[] { 1, 1, 1, new int[] { 1 } }; 
        yield return new object[] { 1, 2, 2, new int[] { 1, 0 } }; 
        yield return new object[] { 2, 1, 1, new int[] { 0 } };    
        yield return new object[] { 3, 1, 0, Array.Empty<int>() };
    }

    [Theory]
    [MemberData(nameof(GetPaginatedTestData))]
    public async Task GetOrderList_Should_Return_Order_List_Correctly(int page, int pageSize, int count, int[] expectedIndexes)
    {
        // Arrange
        var currentUserToken = await CreateCustomerTokenAsync();
        var createProductCommand = CreateProductCommand();
        var productId = await CreateProductAsync(createProductCommand);
        var firstOrder = await CreateOrderAsync(productId, currentUserToken!);
        var secondOrder = await CreateOrderAsync(productId, currentUserToken!);
        var createdOrders = new[] { firstOrder, secondOrder };
        List<Guid> expectedOrderIds = expectedIndexes
            .Select(index => createdOrders[index].Id)
            .ToList();

        var url = $"/api/orders?page={page}&pageSize={pageSize}";
        var request = CreateAuthenticatedRequest(HttpMethod.Get, url, currentUserToken!);

        // Act
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<OrderResponse>>();
        var returnedOrders = result?.Items;
        var returnedOrderIds = returnedOrders?.Select(o => o.Id).ToList();

        // Assert
        Assert.NotNull(returnedOrders);
        Assert.Equal(count, returnedOrders.Count);
        Assert.Equal(expectedOrderIds, returnedOrderIds);
    }


    [Fact]
    public async Task GetOrderList_Should_Return_Empty_List_When_User_Does_Not_Have_Any_Order()
    {
        // Arrange
        var userToken = await CreateCustomerTokenAsync();

        var url = $"/api/orders";
        var request = CreateAuthenticatedRequest(HttpMethod.Get, url, userToken!);

        // Act
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<PaginatedResult<OrderResponse>>();

        // Assert
        Assert.NotNull(orders);
        Assert.Empty(orders.Items);
    }
}
