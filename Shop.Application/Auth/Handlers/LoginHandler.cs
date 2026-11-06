using Shop.Application.Auth.Commands;
using Shop.Application.Interfaces;
using Shop.Application.Exceptions;

namespace Shop.Application.Auth.Handlers;

public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginHandler(IUserRepository repository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _userRepository = repository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<string> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var unauthorizedException = new AuthorizationException("Invalid username or password.");

        var user = await _userRepository.GetByUsernameAsync(command.Username);
        if (user == null)
        {
            throw unauthorizedException;
        }

        var verified = _passwordHasher.Verify(command.Password, user.PasswordHash);
        if (!verified)
        {
            throw unauthorizedException;
        }
        return _jwtTokenService.GenerateToken(user.Id, user.Username, user.Role);
    }
}
