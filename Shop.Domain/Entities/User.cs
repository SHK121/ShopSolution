using Shop.Domain.Enums;

namespace Shop.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    private User() { 
    }

    private User(string username, string passwordHash, UserRole role)
    {
        Id = Guid.NewGuid();
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }

    public static User Create(string username, string passwordHash, UserRole role = UserRole.Customer)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.");

        return new User(username, passwordHash, role);
    }
}
