using Unimag.Inventario.Domain.Purchases;

namespace Unimag.Inventario.Application.Purchases.Dtos;

public sealed record PurchaseItemResponse(Guid ProductId, int Quantity, decimal UnitCost, decimal Total);

public sealed record PurchaseResponse(
    Guid Id,
    Guid? SupplierId,
    PurchaseStatus Status,
    decimal Total,
    DateTime? ReceivedAtUtc,
    Guid CreatedByUserId,
    DateTime CreatedAtUtc,
    IReadOnlyList<PurchaseItemResponse> Items);
