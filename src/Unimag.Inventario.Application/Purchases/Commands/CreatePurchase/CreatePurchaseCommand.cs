namespace Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;

public sealed record CreatePurchaseItem(Guid ProductId, int Quantity, decimal UnitCost);

public sealed record CreatePurchaseCommand(
    Guid TenantId,
    Guid? SupplierId,
    Guid CreatedByUserId,
    IReadOnlyList<CreatePurchaseItem> Items);
