namespace Unimag.Inventario.Api.Platform.Requests;

public sealed record CreatePlatformTenantRequest(string Name, string? AdminEmail, string? AdminPassword);
public sealed record UpdateTenantStatusRequest(string Status);
public sealed record CreateMembershipRequest(string Email, string Password, string Role);
public sealed record UpdateMembershipRequest(string? Role, string? Status);
public sealed record SelectTenantContextRequest(Guid TenantId);
