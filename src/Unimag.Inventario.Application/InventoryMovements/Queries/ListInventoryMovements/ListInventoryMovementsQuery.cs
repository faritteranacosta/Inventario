using Unimag.Inventario.Domain.InventoryMovements;

namespace Unimag.Inventario.Application.InventoryMovements.Queries.ListInventoryMovements;

public sealed record ListInventoryMovementsQuery(
    Guid? ProductId,
    InventoryMovementType? Type,
    DateTime? FromUtc,
    DateTime? ToUtc);
