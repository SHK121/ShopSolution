using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Products.Commands;
using Shop.Infrastructure.Persistence;

namespace Shop.Api.Tests.Integration.Orders;

public class OrderConcurrencyTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public OrderConcurrencyTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SubmitOrder_WhenTwoUsersSubmitLastStock_OneShouldFailWithConflict()
    {
        // 두 사용자가 실제 Submit API를 동시에 호출했을 때, 시스템 전체가 한 명은 성공하고 다른 한 명은 409 Conflict로 처리하는가?
        // 그 mechanism이 실제 API 시나리오에서도 올바르게 동작하는가?
        // Arrange
        var userAToken = await CreateCustomerTokenAsync();
        var userBToken = await CreateCustomerTokenAsync();

        var productId = await CreateProductAsync(new CreateProductCommand("concurrency Test", 100, "USD", 1));

        var orderA = await CreateOrderAsync(productId, userAToken!, quantity: 1);
        var orderB = await CreateOrderAsync(productId, userBToken!, quantity: 1);

        var requestA = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/orders/{orderA.Id}/submit", userAToken!);
        var requestB = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/orders/{orderB.Id}/submit", userBToken!);

        Factory.EnableConcurrencyTest();

        try
        {
            // Act
            var taskA = Client.SendAsync(requestA);
            var taskB = Client.SendAsync(requestB);

            var responses = await Task.WhenAll(taskA, taskB);

            // Assert
            Assert.Contains(
                responses,
                response => response.StatusCode == HttpStatusCode.NoContent);

            Assert.Contains(
                responses,
                response => response.StatusCode == HttpStatusCode.Conflict);

        } finally
        {
            Factory.DisableConcurrencyTest();
        }
        /*
         * 

        // Act
        var taskA = Client.SendAsync(requestA);
        var taskB = Client.SendAsync(requestB);

        await Task.WhenAll(taskA, taskB);

        var responseA = await taskA;
        var responseB = await taskB;
         * var responses = new[] { responseA, responseB };

        // Assert
        Assert.Contains(responses, response => response.StatusCode == HttpStatusCode.NoContent);
        Assert.Contains(responses, response => response.StatusCode == HttpStatusCode.Conflict*/
    }

    [Fact]
    public async Task Product_Update_With_Stale_RowVersion_Should_Throw_ConcurrencyException()
    {
        // 우리 EF Core + SQL Server의 RowVersion concurrency 설정이 실제로 충돌을 감지하는가?
        // Concurrency mechanism이 작동하는가?
        // Arrange
        var productId = await CreateProductAsync(new CreateProductCommand("Test", 100, "USD", 10, "Concurrency Test"));
        using var scopeA = Factory.Services.CreateScope();
        using var scopeB = Factory.Services.CreateScope();

        var contextA = scopeA.ServiceProvider.GetRequiredService<ShopDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<ShopDbContext>();

        var productA = await contextA.Products.FirstAsync(p => p.Id == productId);
        var productB = await contextB.Products.FirstAsync(p => p.Id == productId);

        // 두 context가 같은 버전을 읽었는지 확인
        Assert.Equal(productA.RowVersion, productB.RowVersion);

        // Act
        productA.DecreaseStock(1);
        productB.DecreaseStock(1);

        // 첫 번째 요청은 정상 저장
        await contextA.SaveChangesAsync();

        // 두 번째 Context는 오래된 RowVersion을 가지고 있으므로 실패
        var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            async () =>
            {
                await contextB.SaveChangesAsync();
            });

        // Assert
        Assert.NotNull(exception);

        using var verifyScope = Factory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ShopDbContext>();

        var product = await verifyContext.Products.FirstAsync(p => p.Id == productId);
        Assert.Equal(9, product.Stock);
    }
}
