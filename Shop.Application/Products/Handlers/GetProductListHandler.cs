using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Application.Products.Queries;
using Shop.Application.Common.Pagination;

namespace Shop.Application.Products.Handlers;

public class GetProductListHandler
{
    private readonly IProductRepository _repository;

    public GetProductListHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResult<Product>> Handle(GetProductListQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(query.Pagination.Page, query.Pagination.PageSize, cancellationToken);
    }
}
