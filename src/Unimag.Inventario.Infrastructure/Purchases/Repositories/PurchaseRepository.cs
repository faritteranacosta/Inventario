using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Purchases;
using Unimag.Inventario.Domain.Purchases.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Purchases.Repositories;

public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public PurchaseRepository(AppDbContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Purchases.Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == _currentTenant.TenantId, cancellationToken);

    public async Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Purchases.Include(p => p.Items).AsNoTracking()
            .Where(p => p.TenantId == _currentTenant.TenantId)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<(IReadOnlyList<Purchase> Items, int TotalItems)> GetPageAsync(
        int skip, int take, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Purchases.Include(p => p.Items).AsNoTracking()
            .Where(p => p.TenantId == _currentTenant.TenantId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(p => p.CreatedAtUtc).ThenBy(p => p.Id)
            .Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default) =>
        _dbContext.Purchases.AddAsync(purchase, cancellationToken).AsTask();

    public Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        _dbContext.Purchases.Update(purchase);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
