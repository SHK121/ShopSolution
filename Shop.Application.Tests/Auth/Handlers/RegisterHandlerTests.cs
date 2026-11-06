using Moq;
using Shop.Application.Interfaces;
using Shop.Application.Auth.Handlers;
using Shop.Application.Auth.Commands;
using Shop.Domain.Entities;
using Shop.Application.Exceptions;

namespace Shop.Application.Tests.Auth.Handlers;

public class RegisterHandlerTests
{
    [Fact]
    public async Task Handle_Should_Throw_ConflictException()
    {
        // Arrange
        var mockedRepo = new Mock<IUserRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();
        var mockedHasher = new Mock<IPasswordHasher>();

        var existingUserName = "user1";
        var handler = new RegisterHandler(mockedRepo.Object, mockedUOW.Object, mockedHasher.Object);
        var existingUser = User.Create(existingUserName, "hashed1234");
        mockedRepo.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        // Act and assert
        await Assert.ThrowsAsync<ConflictException>(() =>  handler.Handle(new RegisterCommand(existingUserName, "1234"), CancellationToken.None));

        mockedRepo.Verify(
            x => x.AddAsync(It.IsAny<User>()),
            Times.Never);

        mockedUOW.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);

        mockedHasher.Verify(
            x => x.Hash(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Add_New_User()
    {
        // Arrange
        string username = "NewUser";
        string hashedPW = "hashed1234";
        User? savedUser = null;
        var mockedRepo = new Mock<IUserRepository>();
        var mockedUOW = new Mock<IUnitOfWork>();
        var mockedHasher = new Mock<IPasswordHasher>();

        var handler = new RegisterHandler(mockedRepo.Object, mockedUOW.Object, mockedHasher.Object);
        mockedRepo.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        mockedRepo
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => savedUser = u)
            .Returns(Task.CompletedTask);
        mockedHasher
            .Setup(x => x.Hash(It.IsAny<string>())).Returns(hashedPW);

        // Act
        await handler.Handle(new RegisterCommand(username, "1234"), CancellationToken.None);

        // Act and assert
        mockedRepo.Verify(
            x => x.AddAsync(It.IsAny<User>()),
            Times.Once);
        mockedUOW.Verify(
           x => x.SaveChangesAsync(),
           Times.Once);
        mockedHasher.Verify(
            x => x.Hash(It.IsAny<string>()),
            Times.Once);
        mockedRepo.Verify(
            x => x.GetByUsernameAsync(username),
            Times.Once);

        Assert.NotNull(savedUser);
        Assert.Equal(username, savedUser.Username);
        Assert.Equal(hashedPW, savedUser.PasswordHash);
    }
}
