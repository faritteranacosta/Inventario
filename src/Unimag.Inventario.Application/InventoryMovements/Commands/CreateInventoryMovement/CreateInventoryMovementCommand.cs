using Unimag.Inventario.Domain.InventoryMovements;

namespace Unimag.Inventario.Application.InventoryMovements.Commands.CreateInventoryMovement;

public sealed record CreateInventoryMovementCommand(
    Guid TenantId,
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity,
    string Reason,
    string? ReferenceType,
    Guid? ReferenceId,
    Guid CreatedByUserId);
