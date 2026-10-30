using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Tenants.Repositories;

public sealed class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _db;
    public TenantRepository(AppDbContext db) => _db = db;

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken ct = default)
        => await _db.Tenants.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default)
        => await _db.Tenants.AddAsync(tenant, ct);

    public Task UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        _db.Tenants.Update(tenant);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Tenant tenant, CancellationToken ct = default)
    {
        _db.Tenants.Remove(tenant);
        return Task.CompletedTask;
    }

    public async Task DeleteGraphAsync(Guid tenantId, CancellationToken ct = default)
    {
        _db.TenantMemberships.RemoveRange(_db.TenantMemberships.Where(m => m.TenantId == tenantId));
        _db.Categories.RemoveRange(_db.Categories.Where(c => c.TenantId == tenantId));
        _db.Products.RemoveRange(_db.Products.Where(p => p.TenantId == tenantId));
        _db.Sales.RemoveRange(_db.Sales.Where(s => s.TenantId == tenantId));
        _db.Users.RemoveRange(_db.Users.Where(u => u.TenantId == tenantId));
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct);
        if (tenant is not null)
            _db.Tenants.Remove(tenant);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}