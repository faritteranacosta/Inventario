using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class InvalidMinimumStockException : DomainException
{
    public InvalidMinimumStockException() : base("Minimum stock cannot be negative.")
    {
    }
}
