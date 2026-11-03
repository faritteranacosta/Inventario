using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Purchases.Exceptions;

public sealed class PurchaseItemsRequiredException : DomainException
{
    public PurchaseItemsRequiredException() : base("A purchase must contain at least one item.")
    {
    }
}
