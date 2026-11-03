using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Products;

public sealed class Product : BaseEntity
{
    public Guid TenantId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public Guid CategoryId { get; private set; }

    public string? ImageUrl { get; private set; }

    public int? MinimumStock { get; private set; }

    public string? Barcode { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Product()
    {
        // Required by EF Core materialization.
    }

    private Product(Guid id, Guid tenantId, string name, string? description, decimal price, int stock, Guid categoryId, string? imageUrl, int? minimumStock, string? barcode)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        MinimumStock = minimumStock;
        Barcode = NormalizeBarcode(barcode);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Product Create(Guid tenantId, string name, string? description, decimal price, int stock, Guid categoryId, string? imageUrl = null, int? minimumStock = null, string? barcode = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ProductNameRequiredException();
        }

        if (price < 0)
        {
            throw new InvalidPriceException();
        }

        if (stock < 0)
        {
            throw new InvalidPriceException();
        }

        ValidateMinimumStock(minimumStock);
        return new Product(Guid.NewGuid(), tenantId, name.Trim(), description?.Trim(), price, stock, categoryId, imageUrl, minimumStock, barcode);
    }

    public void UpdateDetails(string name, string? description, decimal price, int stock, Guid categoryId, int? minimumStock = null, string? barcode = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ProductNameRequiredException();
        }

        if (price < 0)
        {
            throw new InvalidPriceException();
        }

        if (stock < 0)
        {
            throw new InvalidPriceException();
        }

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        ValidateMinimumStock(minimumStock);
        MinimumStock = minimumStock;
        Barcode = NormalizeBarcode(barcode);
    }

    public void SetImage(string? imageUrl)
    {
        ImageUrl = imageUrl?.Trim();
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidStockQuantityException();
        }

        if (Stock < quantity)
        {
            throw new InsufficientStockException(Id, quantity, Stock);
        }

        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidStockQuantityException();
        }

        Stock += quantity;
    }

    private static void ValidateMinimumStock(int? minimumStock)
    {
        if (minimumStock < 0)
        {
            throw new InvalidMinimumStockException();
        }
    }

    private static string? NormalizeBarcode(string? barcode)
    {
        var normalized = barcode?.Trim();
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}