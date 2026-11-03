using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Purchases.Exceptions;

namespace Unimag.Inventario.Domain.Purchases;

public sealed class Purchase : BaseEntity
{
    private readonly List<PurchaseItem> _items = new();

    public Guid TenantId { get; private set; }
    public Guid? SupplierId { get; private set; }
    public PurchaseStatus Status { get; private set; }
    public decimal Total => _items.Sum(item => item.Total);
    public DateTime? ReceivedAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public IReadOnlyList<PurchaseItem> Items => _items;

    private Purchase()
    {
    }

    private Purchase(Guid tenantId, Guid? supplierId, Guid createdByUserId, IEnumerable<PurchaseItem> items)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        SupplierId = supplierId;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = DateTime.UtcNow;
        Status = PurchaseStatus.Draft;
        _items.AddRange(items);
    }

    public static Purchase Create(
        Guid tenantId,
        Guid? supplierId,
        Guid createdByUserId,
        IEnumerable<PurchaseItem> items)
    {
        var list = items?.ToList() ?? [];
        if (list.Count == 0)
        {
            throw new PurchaseItemsRequiredException();
        }

        return new Purchase(tenantId, supplierId, createdByUserId, list);
    }

    public void Receive()
    {
        if (Status != PurchaseStatus.Draft)
        {
            throw new PurchaseStatusTransitionException(Status, PurchaseStatus.Received);
        }

        Status = PurchaseStatus.Received;
        ReceivedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == PurchaseStatus.Received)
        {
            throw new PurchaseStatusTransitionException(Status, PurchaseStatus.Cancelled);
        }

        Status = PurchaseStatus.Cancelled;
    }
}
