using Microsoft.EntityFrameworkCore;
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
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}