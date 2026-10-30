using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Users;

public enum UserRole { Admin = 0, User = 1, PlatformAdmin = 2 }

public sealed class User : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private User() { }

    private User(Guid id, Guid tenantId, string email, string passwordHash, UserRole role)
    {
        Id = id;
        TenantId = tenantId;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static User Create(Guid tenantId, string email, string passwordHash, UserRole role = UserRole.User)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new UserEmailRequiredException();
        return new User(Guid.NewGuid(), tenantId, email.Trim().ToLowerInvariant(), passwordHash, role);
    }

    public void ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new UserEmailRequiredException();
        Email = email.Trim().ToLowerInvariant();
    }

    public void ChangePassword(string passwordHash) => PasswordHash = passwordHash;
}

public sealed class UserEmailRequiredException : DomainException
{
    public UserEmailRequiredException() : base("User email cannot be empty.") { }
}

public sealed class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid id) : base($"User '{id}' was not found.") { }
}

public sealed class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string email) : base($"User with email '{email}' already exists.") { }
}

public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() : base("Invalid email or password.") { }
}