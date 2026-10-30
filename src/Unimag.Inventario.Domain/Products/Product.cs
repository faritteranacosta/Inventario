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

    public DateTime CreatedAtUtc { get; private set; }

    private Product()
    {
        // Required by EF Core materialization.
    }

    private Product(Guid id, Guid tenantId, string name, string? description, decimal price, int stock, Guid categoryId, string? imageUrl)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Product Create(Guid tenantId, string name, string? description, decimal price, int stock, Guid categoryId, string? imageUrl = null)
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

        return new Product(Guid.NewGuid(), tenantId, name.Trim(), description?.Trim(), price, stock, categoryId, imageUrl);
    }

    public void UpdateDetails(string name, string? description, decimal price, int stock, Guid categoryId)
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
    }

    public void SetImage(string? imageUrl)
    {
        ImageUrl = imageUrl?.Trim();
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            return;
        }

        if (Stock < quantity)
        {
            throw new InsufficientStockException(Id, quantity, Stock);
        }

        Stock -= quantity;
    }
}