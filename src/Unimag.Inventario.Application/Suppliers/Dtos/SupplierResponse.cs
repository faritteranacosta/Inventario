namespace Unimag.Inventario.Application.Suppliers.Dtos;

public sealed record SupplierResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    bool IsActive,
    DateTime CreatedAtUtc);
