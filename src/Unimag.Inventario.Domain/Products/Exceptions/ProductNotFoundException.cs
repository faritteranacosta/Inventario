using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products.Exceptions;

public sealed class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(Guid id) : base($"Product '{id}' was not found.")
    {
    }
}