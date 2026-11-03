using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Suppliers.Exceptions;

public sealed class SupplierNotFoundException : DomainException
{
    public SupplierNotFoundException(Guid id) : base($"Supplier '{id}' was not found.")
    {
    }
}
