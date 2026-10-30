using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Samples.Exceptions;

/// <summary>
/// Thrown when a lookup for a Sample by id finds nothing. Second reference
/// example alongside SampleNameRequiredException: same base class, own
/// fixed message shape, different HTTP status once it reaches
/// ExceptionHandlingMiddleware.
/// </summary>
public sealed class SampleNotFoundException : DomainException
{
    public SampleNotFoundException(Guid id) : base($"Sample '{id}' was not found.")
    {
    }
}
