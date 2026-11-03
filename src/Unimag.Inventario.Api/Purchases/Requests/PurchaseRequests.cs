using Unimag.Inventario.Domain.Purchases;

namespace Unimag.Inventario.Api.Purchases.Requests;

public sealed record CreatePurchaseItemRequest(Guid ProductId, int Quantity, decimal UnitCost);

public sealed record CreatePurchaseRequest(
    Guid? SupplierId,
    IReadOnlyList<CreatePurchaseItemRequest> Items);

public sealed record ChangePurchaseStatusRequest(PurchaseStatus Status);
