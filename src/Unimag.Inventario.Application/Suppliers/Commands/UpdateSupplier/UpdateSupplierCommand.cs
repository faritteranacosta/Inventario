namespace Unimag.Inventario.Application.Suppliers.Commands.UpdateSupplier;

public sealed record UpdateSupplierCommand(Guid Id, string Name, string? Email, string? Phone);
