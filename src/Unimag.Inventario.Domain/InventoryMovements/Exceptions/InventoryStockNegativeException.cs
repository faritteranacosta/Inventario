using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.InventoryMovements.Exceptions;

public sealed class InventoryStockNegativeException : DomainException
{
    public InventoryStockNegativeException() : base("Inventory stock cannot be negative.")
    {
    }
}
