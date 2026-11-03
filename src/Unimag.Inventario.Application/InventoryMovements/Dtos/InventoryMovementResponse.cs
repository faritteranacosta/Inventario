using Unimag.Inventario.Domain.InventoryMovements;

namespace Unimag.Inventario.Application.InventoryMovements.Dtos;

public sealed record InventoryMovementResponse(
    Guid Id,
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity,
    int StockBefore,
    int StockAfter,
    string Reason,
    string? ReferenceType,
    Guid? ReferenceId,
    Guid CreatedByUserId,
    DateTime CreatedAtUtc);
