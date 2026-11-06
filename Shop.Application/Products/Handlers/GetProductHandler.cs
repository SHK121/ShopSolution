using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Application.Exceptions;
using Shop.Application.Products.Queries;

namespace Shop.Application.Products.Handlers;

public class GetProductHandler
{
    private readonly IProductRepository _repository;

    public GetProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await _repository.GetAsync(query.Id, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {query.Id} not found.");
        }
        return product;
    }
}
