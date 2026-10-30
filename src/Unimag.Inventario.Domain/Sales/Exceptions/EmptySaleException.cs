using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Sales.Exceptions;

public sealed class EmptySaleException : DomainException
{
    public EmptySaleException() : base("A sale must contain at least one item.")
    {
    }
}