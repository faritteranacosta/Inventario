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

    public Task<(IReadOnlyList<Product> Items, int TotalItems)> GetPageAsync(
        Guid? categoryId, string? search, int skip, int take,
        CancellationToken cancellationToken = default)
    {
        var query = _store.Values.Where(p => p.TenantId == _tenant.TenantId);
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));
        var ordered = query.OrderBy(p => p.CreatedAtUtc).ThenBy(p => p.Id).ToList();
        return Task.FromResult<(IReadOnlyList<Product>, int)>(
            (ordered.Skip(skip).Take(take).ToList(), ordered.Count));
    }

    public Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Values.FirstOrDefault(p =>
            p.TenantId == _tenant.TenantId && p.Barcode == barcode.Trim()));

    public Task<IReadOnlyList<Product>> GetLowStockAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Product>>(_store.Values.Where(p =>
            p.TenantId == _tenant.TenantId &&
            (p.Stock == 0 || (p.MinimumStock.HasValue && p.Stock <= p.MinimumStock.Value))).ToList());

    public Task<bool> BarcodeExistsAsync(string barcode, Guid? excludingProductId = null, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Values.Any(p =>
            p.TenantId == _tenant.TenantId &&
            p.Barcode == barcode.Trim() &&
            (!excludingProductId.HasValue || p.Id != excludingProductId.Value)));

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