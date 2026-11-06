using Microsoft.EntityFrameworkCore;
using Shop.Application.Common.Pagination;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopDbContext _context;

    public ProductRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
        /*Product (Change Tracker)
            State = Deleted*/
    }

    public async Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Product>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        // return await _context.Products.ToListAsync();
        var items = await _context.Products.Skip((page - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync(cancellationToken);
        var totalCount = await _context.Products.CountAsync();
        var result = new PaginatedResult<Product>(items, page, pageSize, totalCount);

        return result;
    }

   /* public Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);

        return Task.CompletedTask;
    }*/
        }
