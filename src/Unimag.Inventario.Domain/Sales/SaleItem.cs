namespace Unimag.Inventario.Domain.Sales;

public sealed class SaleItem
{
    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Total => UnitPrice * Quantity;

    private SaleItem()
    {
        // Required by EF Core materialization.
    }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        }

        ProductId = productId;
        ProductName = (productName ?? string.Empty).Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}