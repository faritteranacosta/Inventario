namespace Unimag.Inventario.Application.Tenants.Dtos;
public sealed record TenantResponse(Guid Id, string Name, string DisplayName, string? LogoUrl, DateTime CreatedAtUtc);