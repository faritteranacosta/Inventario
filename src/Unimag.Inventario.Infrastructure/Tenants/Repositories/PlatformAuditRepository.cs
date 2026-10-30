using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Tenants.Repositories;

public sealed class PlatformAuditRepository : IPlatformAuditRepository
{
    private readonly AppDbContext _db;
    public PlatformAuditRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(PlatformAuditEvent auditEvent, CancellationToken ct = default)
        => await _db.PlatformAuditEvents.AddAsync(auditEvent, ct);

    public async Task<IReadOnlyList<PlatformAuditEvent>> GetAllAsync(CancellationToken ct = default)
        => await _db.PlatformAuditEvents.AsNoTracking().OrderByDescending(e => e.CreatedAtUtc).ToListAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
