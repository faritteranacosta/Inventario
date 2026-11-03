using Unimag.Inventario.Domain.InventoryMovements;

namespace Unimag.Inventario.Api.InventoryMovements.Requests;

public sealed record CreateInventoryMovementRequest(
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity,
    string Reason,
    string? ReferenceType,
    Guid? ReferenceId);
