namespace Unimag.Inventario.Domain.Common;

/// <summary>
/// Base type for every business-rule violation raised by the Domain.
/// Abstract on purpose: nobody throws `DomainException` itself — each
/// invariant gets its own concrete subclass living in
/// Domain/{Dominio}/Exceptions/{Nombre}Exception.cs, passing its own fixed
/// message to `base(...)` (see
/// Domain/Samples/Exceptions/SampleNameRequiredException.cs and
/// SampleNotFoundException.cs). Use cases don't catch these to build a
/// Result — they just let them propagate; ExceptionHandlingMiddleware maps
/// the concrete exception type to the right HTTP status code and sends the
/// message back as-is.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}
