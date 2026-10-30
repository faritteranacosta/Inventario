using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Users;
using Unimag.Inventario.Domain.Users.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Users.Repositories;

public sealed class TenantMembershipRepository : ITenantMembershipRepository
{
    private readonly AppDbContext _db;
    public TenantMembershipRepository(AppDbContext db) => _db = db;

    public async Task<TenantMembership?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.TenantMemberships.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<TenantMembership?> GetAsync(Guid userId, Guid tenantId, CancellationToken ct = default)
        => await _db.TenantMemberships.FirstOrDefaultAsync(m => m.UserId == userId && m.TenantId == tenantId, ct);

    public async Task<IReadOnlyList<TenantMembership>> GetByTenantIdAsync(Guid tenantId, CancellationToken ct = default)
        => await _db.TenantMemberships.AsNoTracking().Where(m => m.TenantId == tenantId).ToListAsync(ct);

    public async Task<IReadOnlyList<TenantMembership>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.TenantMemberships.AsNoTracking().Where(m => m.UserId == userId).ToListAsync(ct);

    public async Task AddAsync(TenantMembership membership, CancellationToken ct = default)
        => await _db.TenantMemberships.AddAsync(membership, ct);

    public Task RemoveAsync(TenantMembership membership, CancellationToken ct = default)
    {
        _db.TenantMemberships.Remove(membership);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
