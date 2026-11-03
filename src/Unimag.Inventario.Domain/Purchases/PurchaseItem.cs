using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Purchases.Exceptions;

namespace Unimag.Inventario.Domain.Purchases;

public sealed class PurchaseItem : BaseEntity
{
    public Guid PurchaseId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal Total => Quantity * UnitCost;

    private PurchaseItem()
    {
    }

    private PurchaseItem(Guid productId, int quantity, decimal unitCost)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
    }

    public static PurchaseItem Create(Guid productId, int quantity, decimal unitCost)
    {
        if (quantity <= 0 || unitCost < 0)
        {
            throw new InvalidPurchaseItemException();
        }

        return new PurchaseItem(productId, quantity, unitCost);
    }
}
