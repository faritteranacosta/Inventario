namespace Unimag.Inventario.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ITokenService
{
    string GenerateToken(
        Unimag.Inventario.Domain.Users.User user,
        Unimag.Inventario.Domain.Tenants.Tenant? tenant,
        Unimag.Inventario.Domain.Users.UserRole? activeRole = null);
}