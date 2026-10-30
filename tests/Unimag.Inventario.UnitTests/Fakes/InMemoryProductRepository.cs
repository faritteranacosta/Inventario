using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Products;
using Unimag.Inventario.Domain.Products.Contracts;

namespace Unimag.Inventario.UnitTests.Fakes;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _store = new();
    private readonly ICurrentTenant _tenant;

    public InMemoryProductRepository(ICurrentTenant tenant)
    {
        _tenant = tenant;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryGetValue(id, out var p) && p.TenantId == _tenant.TenantId ? p : null);

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Product>>(_store.Values.Where(p => p.TenantId == _tenant.TenantId).ToList());

    public Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Product>>(_store.Values.Where(p => p.CategoryId == categoryId && p.TenantId == _tenant.TenantId).ToList());

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _store[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _store[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _store.Remove(product.Id);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}