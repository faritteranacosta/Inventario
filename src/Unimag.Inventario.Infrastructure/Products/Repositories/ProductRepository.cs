using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Common.Exceptions;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Products;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Products.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public ProductRepository(AppDbContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id && p.TenantId == _currentTenant.TenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.AsNoTracking().Where(p => p.TenantId == _currentTenant.TenantId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.AsNoTracking()
            .Where(p => p.CategoryId == categoryId && p.TenantId == _currentTenant.TenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalItems)> GetPageAsync(
        Guid? categoryId, string? search, int skip, int take,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products.AsNoTracking()
            .Where(p => p.TenantId == _currentTenant.TenantId);
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p => p.Name.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(p => p.CreatedAtUtc).ThenBy(p => p.Id)
            .Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        var normalized = barcode.Trim();
        return await _dbContext.Products.FirstOrDefaultAsync(
            p => p.TenantId == _currentTenant.TenantId && p.Barcode == normalized,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.AsNoTracking()
            .Where(p => p.TenantId == _currentTenant.TenantId &&
                        ((p.Stock == 0) || (p.MinimumStock.HasValue && p.Stock <= p.MinimumStock.Value)))
            .OrderBy(p => p.Stock)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> BarcodeExistsAsync(string barcode, Guid? excludingProductId = null, CancellationToken cancellationToken = default)
    {
        var normalized = barcode.Trim();
        return _dbContext.Products.AnyAsync(
            p => p.TenantId == _currentTenant.TenantId &&
                 p.Barcode == normalized &&
                 (!excludingProductId.HasValue || p.Id != excludingProductId.Value),
            cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Remove(product);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException();
        }
    }
}