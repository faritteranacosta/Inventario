using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Purchases.Exceptions;

public sealed class InvalidPurchaseItemException : DomainException
{
    public InvalidPurchaseItemException() : base("Purchase quantity must be greater than zero and unit cost cannot be negative.")
    {
    }
}
