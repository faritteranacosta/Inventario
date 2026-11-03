using Unimag.Inventario.Domain.Products;

namespace Unimag.Inventario.Domain.Products.Contracts;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Product> Items, int TotalItems)> GetPageAsync(
        Guid? categoryId, string? search, int skip, int take,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetLowStockAsync(CancellationToken cancellationToken = default);

    Task<bool> BarcodeExistsAsync(string barcode, Guid? excludingProductId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}