using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Sales.Exceptions;

public sealed class IdempotencyKeyPayloadMismatchException : DomainException
{
    public IdempotencyKeyPayloadMismatchException(string key)
        : base($"Idempotency key '{key}' was already used with a different payload.")
    {
    }
}
