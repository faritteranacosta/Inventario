namespace Unimag.Inventario.Domain.Sales;

public sealed class SaleItem
{
    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Total => UnitPrice * Quantity;

    private SaleItem()
    {
        // Required by EF Core materialization.
    }

    public SaleItem(Guid productId, int quantity, decimal unitPrice)
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
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}