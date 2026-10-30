using Unimag.Inventario.Domain.Tenants;

namespace Unimag.Inventario.Domain.Tenants.Contracts;

public interface IPlatformAuditRepository
{
    Task AddAsync(PlatformAuditEvent auditEvent, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlatformAuditEvent>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
