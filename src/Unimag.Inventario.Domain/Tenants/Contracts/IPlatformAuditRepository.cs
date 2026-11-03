using Unimag.Inventario.Domain.Tenants;

namespace Unimag.Inventario.Domain.Tenants.Contracts;

public interface IPlatformAuditRepository
{
    Task AddAsync(PlatformAuditEvent auditEvent, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<PlatformAuditEvent> Items, int TotalItems)> GetPageAsync(
        string? action, string? resourceType, DateTime? fromUtc, DateTime? toUtc,
        int skip, int take, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
