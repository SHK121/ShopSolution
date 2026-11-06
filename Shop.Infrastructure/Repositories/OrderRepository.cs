using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Infrastructure.Persistence;
using Shop.Application.Common.Pagination;

namespace Shop.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ShopDbContext _context;

    public OrderRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
    }

    public async Task<PaginatedResult<Order>> GetByUserIdAsync(Guid userId, 
        CancellationToken cancellationToken,
        int page,
        int pageSize)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            // "이 Entity를 DB에서 읽기만 할 거니까 EF Core가 변경 추적할 필요 없어."
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var totalCount = await _context.Orders.CountAsync(o => o.UserId == userId);
        var result = new PaginatedResult<Order>(orders, page, pageSize, totalCount);

        return result;
    }
}
