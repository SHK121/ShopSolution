using Shop.Api.Contracts;
using Shop.Domain.Entities;

namespace Shop.Api.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToResponse(
       this Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Price.Amount,
            product.Price.Currency.ToString(),
            product.Stock,
            product.Description);
    }
}
