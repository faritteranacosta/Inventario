using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.InventoryMovements.Exceptions;

public sealed class InventoryMovementNotFoundException : DomainException
{
    public InventoryMovementNotFoundException(Guid id) : base($"Inventory movement '{id}' was not found.")
    {
    }
}
