using Shop.Application.Common.Pagination;
using Shop.Domain.Entities;

namespace Shop.Application.Interfaces;

public interface IProductRepository {
    Task AddAsync(Product product);
    void Remove(Product product);
    Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    //Task UpdateAsync(Product product);
    Task<PaginatedResult<Product>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
}