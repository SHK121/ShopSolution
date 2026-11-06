using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Infrastructure.Persistence;

namespace Shop.Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly ShopDbContext _context;

    public UserRepository(ShopDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(String username)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
    }
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}
