using Shop.Application.Auth.Commands;
using Shop.Application.Interfaces;
using Shop.Application.Exceptions;
using Shop.Domain.Entities;

namespace Shop.Application.Auth.Handlers;

public class RegisterHandler
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterHandler(IUserRepository repository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var userAlreadyExists = await _repository.GetByUsernameAsync(command.Username);
        if (userAlreadyExists != null)
        {
            throw new ConflictException("The username already exists.");
        }

        var user = User.Create(command.Username, _passwordHasher.Hash(command.Password));
        await _repository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
