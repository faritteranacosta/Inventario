namespace Unimag.Inventario.Api.Auth.Requests;
public sealed record LoginRequest(string Email, string Password);
public sealed record RegisterRequest(Guid TenantId, string Email, string Password, string Role);
public sealed record UpdateProfileRequest(string Email);
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);