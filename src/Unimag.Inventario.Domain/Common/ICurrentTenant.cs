namespace Unimag.Inventario.Domain.Common;

/// <summary>
/// Provides the tenant identifier of the current request. Implemented in the
/// Api layer (from the JWT claim) and consumed by Infrastructure repositories
/// to enforce tenant isolation.
/// </summary>
public interface ICurrentTenant
{
    Guid TenantId { get; }
}