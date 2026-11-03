using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Sales.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Sales.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public SaleRepository(AppDbContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == _currentTenant.TenantId, cancellationToken);
    }

    public Task<Sale?> GetByIdempotencyKeyAsync(string key, CancellationToken cancellationToken = default) =>
        _dbContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(
                s => s.TenantId == _currentTenant.TenantId && s.IdempotencyKey == key,
                cancellationToken);

    public async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.TenantId == _currentTenant.TenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.TenantId == _currentTenant.TenantId && s.SaleDateUtc >= fromUtc && s.SaleDateUtc <= toUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Sale> Items, int TotalItems)> GetPageAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Sales
            .Include(s => s.Items)
            .AsNoTracking()
            .Where(s => s.TenantId == _currentTenant.TenantId);
        if (fromUtc.HasValue)
            query = query.Where(s => s.SaleDateUtc >= fromUtc.Value);
        if (toUtc.HasValue)
            query = query.Where(s => s.SaleDateUtc <= toUtc.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(s => s.SaleDateUtc).ThenBy(s => s.Id)
            .Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _dbContext.Sales.AddAsync(sale, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}