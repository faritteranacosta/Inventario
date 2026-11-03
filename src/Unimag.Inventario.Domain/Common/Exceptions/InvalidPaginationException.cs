using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Common.Exceptions;

public sealed class InvalidPaginationException : DomainException
{
    public InvalidPaginationException() : base("Page must be greater than zero and page size must be between 1 and 100.")
    {
    }
}
