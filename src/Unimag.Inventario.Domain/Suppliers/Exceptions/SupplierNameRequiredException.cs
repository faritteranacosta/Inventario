using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Suppliers.Exceptions;

public sealed class SupplierNameRequiredException : DomainException
{
    public SupplierNameRequiredException() : base("Supplier name cannot be empty.")
    {
    }
}
