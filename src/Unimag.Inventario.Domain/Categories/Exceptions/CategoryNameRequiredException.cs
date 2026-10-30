using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Categories.Exceptions;

public sealed class CategoryNameRequiredException : DomainException
{
    public CategoryNameRequiredException() : base("Category name cannot be empty.")
    {
    }
}