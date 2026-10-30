using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class ProductNameRequiredException : DomainException
{
    public ProductNameRequiredException() : base("Product name cannot be empty.")
    {
    }
}