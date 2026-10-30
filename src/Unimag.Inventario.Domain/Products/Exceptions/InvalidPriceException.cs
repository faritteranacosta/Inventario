using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class InvalidPriceException : DomainException
{
    public InvalidPriceException() : base("Product price cannot be negative.")
    {
    }
}