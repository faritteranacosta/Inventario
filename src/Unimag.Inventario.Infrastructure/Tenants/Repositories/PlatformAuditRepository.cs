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

    public async Task<(IReadOnlyList<PlatformAuditEvent> Items, int TotalItems)> GetPageAsync(
        string? action, string? resourceType, DateTime? fromUtc, DateTime? toUtc,
        int skip, int take, CancellationToken ct = default)
    {
        var query = _db.PlatformAuditEvents.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(e => e.Action == action);
        if (!string.IsNullOrWhiteSpace(resourceType))
            query = query.Where(e => e.ResourceType == resourceType);
        if (fromUtc.HasValue)
            query = query.Where(e => e.CreatedAtUtc >= fromUtc.Value);
        if (toUtc.HasValue)
            query = query.Where(e => e.CreatedAtUtc <= toUtc.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(e => e.CreatedAtUtc)
            .Skip(skip).Take(take).ToListAsync(ct);
        return (items, total);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
