using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Purchases.Exceptions;

public sealed class PurchaseNotFoundException : DomainException
{
    public PurchaseNotFoundException(Guid id) : base($"Purchase '{id}' was not found.")
    {
    }
}
