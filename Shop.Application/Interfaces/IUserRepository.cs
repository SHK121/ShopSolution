using Shop.Domain.Entities;

namespace Shop.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(String username);
    Task AddAsync(User user);
}
