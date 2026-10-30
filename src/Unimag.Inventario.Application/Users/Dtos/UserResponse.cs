namespace Unimag.Inventario.Application.Users.Dtos;
public sealed record UserResponse(Guid Id, Guid TenantId, string Email, string Role, DateTime CreatedAtUtc);