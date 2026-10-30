using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Categories.Exceptions;

public sealed class CategoryNotFoundException : DomainException
{
    public CategoryNotFoundException(Guid id) : base($"Category '{id}' was not found.")
    {
    }
}