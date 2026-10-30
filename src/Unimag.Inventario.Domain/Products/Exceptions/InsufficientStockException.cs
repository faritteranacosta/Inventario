using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(Guid id, int requested, int available)
        : base($"Product '{id}' has insufficient stock. Requested {requested}, available {available}.")
    {
    }
}