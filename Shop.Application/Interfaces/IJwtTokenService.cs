using Shop.Domain.Enums;

namespace Shop.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string username, UserRole role);
}
