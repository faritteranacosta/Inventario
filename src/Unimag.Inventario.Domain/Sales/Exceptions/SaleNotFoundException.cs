using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Sales.Exceptions;

public sealed class SaleNotFoundException : DomainException
{
    public SaleNotFoundException(Guid id) : base($"Sale '{id}' was not found.")
    {
    }
}