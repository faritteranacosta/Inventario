using Unimag.Inventario.Api.InventoryMovements.Requests;
using Unimag.Inventario.Application.InventoryMovements.Commands.CreateInventoryMovement;
using Unimag.Inventario.Application.InventoryMovements.Queries.GetInventoryMovementById;
using Unimag.Inventario.Application.InventoryMovements.Queries.ListInventoryMovements;

namespace Unimag.Inventario.Api.InventoryMovements.Mappers;

public static class InventoryMovementMapper
{
    public static CreateInventoryMovementCommand ToCreateCommand(
        Guid tenantId,
        Guid userId,
        CreateInventoryMovementRequest request) =>
        new(tenantId, request.ProductId, request.Type, request.Quantity, request.Reason,
            request.ReferenceType, request.ReferenceId, userId);

    public static GetInventoryMovementByIdQuery ToGetQuery(Guid id) => new(id);

    public static ListInventoryMovementsQuery ToListQuery(
        Guid? productId,
        Unimag.Inventario.Domain.InventoryMovements.InventoryMovementType? type,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize) =>
        new(productId, type, from, to, page, pageSize);
}
