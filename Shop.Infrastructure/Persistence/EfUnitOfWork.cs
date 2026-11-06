using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Persistence;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ShopDbContext _context;

    public EfUnitOfWork(ShopDbContext context)
    {
        _context = context;
        Console.WriteLine(
            _context.Database.GetConnectionString());
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("SaveChanges 시작");

        await _context.SaveChangesAsync(cancellationToken);
    }
}
