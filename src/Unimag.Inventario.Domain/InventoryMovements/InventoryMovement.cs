using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.InventoryMovements.Exceptions;

namespace Unimag.Inventario.Domain.InventoryMovements;

public sealed class InventoryMovement : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid ProductId { get; private set; }
    public InventoryMovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public int StockBefore { get; private set; }
    public int StockAfter { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? ReferenceType { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private InventoryMovement()
    {
    }

    private InventoryMovement(
        Guid tenantId,
        Guid productId,
        InventoryMovementType type,
        int quantity,
        int stockBefore,
        int stockAfter,
        string reason,
        string? referenceType,
        Guid? referenceId,
        Guid createdByUserId)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ProductId = productId;
        Type = type;
        Quantity = quantity;
        StockBefore = stockBefore;
        StockAfter = stockAfter;
        Reason = reason.Trim();
        ReferenceType = referenceType?.Trim();
        ReferenceId = referenceId;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static InventoryMovement Create(
        Guid tenantId,
        Guid productId,
        InventoryMovementType type,
        int quantity,
        int stockBefore,
        string reason,
        string? referenceType,
        Guid? referenceId,
        Guid createdByUserId)
    {
        if (quantity <= 0)
        {
            throw new InvalidMovementQuantityException();
        }

        if (type == InventoryMovementType.Adjustment && string.IsNullOrWhiteSpace(reason))
        {
            throw new MovementReasonRequiredException();
        }

        var stockAfter = type == InventoryMovementType.Out
            ? stockBefore - quantity
            : stockBefore + quantity;

        if (stockAfter < 0)
        {
            throw new InventoryStockNegativeException();
        }

        return new InventoryMovement(
            tenantId, productId, type, quantity, stockBefore, stockAfter,
            reason, referenceType, referenceId, createdByUserId);
    }
}
