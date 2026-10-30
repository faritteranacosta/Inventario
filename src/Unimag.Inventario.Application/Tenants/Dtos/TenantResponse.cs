namespace Unimag.Inventario.Application.Tenants.Dtos;
public sealed record TenantResponse(Guid Id, string Name, DateTime CreatedAtUtc);