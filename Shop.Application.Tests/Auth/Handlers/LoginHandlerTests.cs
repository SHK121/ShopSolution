using Moq;
using Shop.Application.Interfaces;
using Shop.Application.Auth.Handlers;
using Shop.Domain.Entities;
using Shop.Application.Exceptions;
using Shop.Application.Auth.Commands;
using Shop.Domain.Enums;

namespace Shop.Application.Tests.Auth.Handlers;

public class LoginHandlerTests
{
    [Fact]
    public async Task Handle_Should_Throw_Exception_For_Non_Existing_User()
    {
        // Arrange
        var mockedRepo = new Mock<IUserRepository>();
        var mockedHasher = new Mock<IPasswordHasher>();
        var mockedJwtService = new Mock<IJwtTokenService>();

        var handler = new LoginHandler(mockedRepo.Object, mockedHasher.Object, mockedJwtService.Object);
        mockedRepo.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var command = new LoginCommand("user", "pw");

        // Act and assert
        await Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(command, CancellationToken.None));
        mockedHasher.Verify(
            x => x.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        mockedJwtService.Verify(
            x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_For_Wrong_Login_Info()
    {
        // Arrange
        var mockedRepo = new Mock<IUserRepository>();
        var mockedHasher = new Mock<IPasswordHasher>();
        var mockedJwtService = new Mock<IJwtTokenService>();

        var handler = new LoginHandler(mockedRepo.Object, mockedHasher.Object, mockedJwtService.Object);
        var user = User.Create("user", "1234hashed");
        mockedRepo.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
        mockedHasher.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var command = new LoginCommand("user", "pw");

        // Act and assert
        await Assert.ThrowsAsync<AuthorizationException>(() => handler.Handle(command, CancellationToken.None));
        mockedHasher.Verify(
            x => x.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        mockedJwtService.Verify(
            x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Token()
    {
        // Arrange
        var mockedRepo = new Mock<IUserRepository>();
        var mockedHasher = new Mock<IPasswordHasher>();
        var mockedJwtService = new Mock<IJwtTokenService>();
        var expectedToken = "token";
        var username = "user";
        var password = "pw";
        var hashedPW = "1234hashed";

        var handler = new LoginHandler(mockedRepo.Object, mockedHasher.Object, mockedJwtService.Object);
        var user = User.Create(username, hashedPW);
        mockedRepo.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);
        mockedHasher.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        mockedJwtService.Setup(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>())).Returns(expectedToken);
        var command = new LoginCommand(username, password);

        // Act
        var actualResult = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockedHasher.Verify(
            x => x.Verify(password, hashedPW),
            Times.Once);
        mockedJwtService.Verify(
            x => x.GenerateToken(user.Id, username, UserRole.Customer),
            Times.Once);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedToken, actualResult);
    }
}
