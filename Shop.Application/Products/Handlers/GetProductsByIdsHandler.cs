using Shop.Application.Interfaces;
using Shop.Application.Products.Queries;
using Shop.Domain.Entities;

namespace Shop.Application.Products.Handlers;

public class GetProductsByIdsHandler
{
    private readonly IProductRepository _repository;

    public GetProductsByIdsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> Handle(GetProductsByIdsQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdsAsync(query.Ids, cancellationToken);
    }
}
