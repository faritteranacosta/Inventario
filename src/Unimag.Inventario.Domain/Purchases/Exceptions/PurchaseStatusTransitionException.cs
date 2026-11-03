using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Purchases.Exceptions;

public sealed class PurchaseStatusTransitionException : DomainException
{
    public PurchaseStatusTransitionException(PurchaseStatus current, PurchaseStatus target)
        : base($"Purchase cannot change from '{current}' to '{target}'.")
    {
    }
}
