using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.InventoryMovements.Exceptions;

public sealed class MovementReasonRequiredException : DomainException
{
    public MovementReasonRequiredException() : base("A reason is required for inventory adjustments.")
    {
    }
}
