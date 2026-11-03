namespace Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;

public sealed record CreateSupplierCommand(Guid TenantId, string Name, string? Email, string? Phone);
