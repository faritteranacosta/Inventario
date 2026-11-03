using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class InvalidStockQuantityException : DomainException
{
    public InvalidStockQuantityException() : base("Stock quantity must be greater than zero.")
    {
    }
}
