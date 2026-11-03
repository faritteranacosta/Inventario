using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Sales.Exceptions;

namespace Unimag.Inventario.Domain.Sales;

public sealed class Sale : BaseEntity
{
    public Guid TenantId { get; private set; }

    public DateTime SaleDateUtc { get; private set; }

    private readonly List<SaleItem> _items = new();

    public IReadOnlyList<SaleItem> Items => _items;

    public decimal Total => _items.Sum(i => i.Total);

    private Sale()
    {
        // Required by EF Core materialization.
    }

    private Sale(Guid id, Guid tenantId, IEnumerable<SaleItem> items)
    {
        Id = id;
        TenantId = tenantId;
        _items.AddRange(items);
        SaleDateUtc = DateTime.UtcNow;
    }

    public static Sale Create(Guid tenantId, IEnumerable<SaleItem> items)
    {
        var list = items?.ToList() ?? new List<SaleItem>();
        if (list.Count == 0)
        {
            throw new EmptySaleException();
        }

        return new Sale(Guid.NewGuid(), tenantId, list);
    }
}