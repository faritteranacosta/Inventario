using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.InventoryMovements.Exceptions;

public sealed class InvalidMovementQuantityException : DomainException
{
    public InvalidMovementQuantityException() : base("Movement quantity must be greater than zero.")
    {
    }
}
