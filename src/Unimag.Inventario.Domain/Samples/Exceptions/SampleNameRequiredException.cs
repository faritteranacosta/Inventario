using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Samples.Exceptions;

/// <summary>
/// Thrown by Sample.Create/Sample.Rename when the name invariant is
/// violated. Owns its own fixed message — callers never pass one in.
/// </summary>
public sealed class SampleNameRequiredException : DomainException
{
    public SampleNameRequiredException() : base("Sample name cannot be empty.")
    {
    }
}
