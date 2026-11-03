namespace Unimag.Inventario.Api.Suppliers.Requests;

public sealed record CreateSupplierRequest(string Name, string? Email, string? Phone);

public sealed record UpdateSupplierRequest(string Name, string? Email, string? Phone);
