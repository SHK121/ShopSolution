using Shop.Application.Products.Handlers;
using Shop.Application.Products.Queries;
using Shop.Application.Common.Pagination;
using Shop.Domain.Entities;
using Shop.Domain.ValueObjects;
using Moq;
using Shop.Application.Interfaces;

namespace Shop.Application.Tests.Products.Handlers;

public class GetProductListHandlerTests
{
    public static IEnumerable<object[]> GetPaginatedTestData()
    {
        yield return new object[] { 1, 2, new int[] { 0, 1 } }; // Page 1, PageSize 2, Expect first two products
        yield return new object[] { 2, 2, new int[] { 2 } };    // Page 2, PageSize 2, Expect third product
        yield return new object[] { 3, 1, new int[] { 2 } };    // Page 3, PageSize 1, Expect third product
    }
    
    [Theory]
    [MemberData(nameof(GetPaginatedTestData))]
    public async Task Handle_Should_Return_Paginated_Result(int page, int pageSize, int[] expectedIndexes)
    {
        // Arrange
        var query = new GetProductListQuery(new PageRequest(page, pageSize));
        var products = new List<Product>
        {
            Product.Create("Product 1", Money.Create(10, "USD"), 5),
            Product.Create("Product 2", Money.Create(20, "USD"), 10),
            Product.Create("Product 3", Money.Create(30, "USD"), 15)
        };
        var expectedResult = new PaginatedResult<Product>(products.Where((p, i) => expectedIndexes.Contains(i)).ToList(), page, pageSize, products.Count);
        var mockedRepo = new Mock<IProductRepository>();
        mockedRepo.Setup(x => x.GetAllAsync(page, pageSize, CancellationToken.None))
            .ReturnsAsync(expectedResult);
        var handler = new GetProductListHandler(mockedRepo.Object);

        // Act
        var actualResult = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult.Page, actualResult.Page);
        Assert.Equal(expectedResult.PageSize, actualResult.PageSize);
        Assert.Equal(expectedResult.TotalCount, actualResult.TotalCount);
        Assert.Equal(
            expectedResult.Items.Select(x => x.Id),
            actualResult.Items.Select(x => x.Id)
        );
        mockedRepo.Verify(x => x.GetAllAsync(page, pageSize, CancellationToken.None), Times.Once);
    }
}
