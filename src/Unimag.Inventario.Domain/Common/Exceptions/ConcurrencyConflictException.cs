using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Common.Exceptions;

public sealed class ConcurrencyConflictException : DomainException
{
    public ConcurrencyConflictException()
        : base("The resource was changed by another operation. Please retry.")
    {
    }
}
