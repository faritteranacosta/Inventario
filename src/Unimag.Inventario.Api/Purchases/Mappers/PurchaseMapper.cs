using Unimag.Inventario.Api.Purchases.Requests;
using Unimag.Inventario.Application.Purchases.Commands.ChangePurchaseStatus;
using Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;
using Unimag.Inventario.Application.Purchases.Queries.GetPurchaseById;

namespace Unimag.Inventario.Api.Purchases.Mappers;

public static class PurchaseMapper
{
    public static CreatePurchaseCommand ToCreateCommand(
        Guid tenantId,
        Guid userId,
        CreatePurchaseRequest request) =>
        new(tenantId, request.SupplierId, userId,
            request.Items.Select(item =>
                new CreatePurchaseItem(item.ProductId, item.Quantity, item.UnitCost)).ToList());

    public static ChangePurchaseStatusCommand ToChangeStatusCommand(
        Guid id,
        Guid userId,
        ChangePurchaseStatusRequest request) =>
        new(id, request.Status, userId);

    public static GetPurchaseByIdQuery ToGetQuery(Guid id) => new(id);
}
