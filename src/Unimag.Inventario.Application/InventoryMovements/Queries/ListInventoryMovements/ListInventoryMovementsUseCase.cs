using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.InventoryMovements.Dtos;
using Unimag.Inventario.Domain.InventoryMovements;
using Unimag.Inventario.Domain.InventoryMovements.Contracts;

namespace Unimag.Inventario.Application.InventoryMovements.Queries.ListInventoryMovements;

public sealed class ListInventoryMovementsUseCase
    : IUseCase<ListInventoryMovementsQuery, IReadOnlyList<InventoryMovementResponse>>
{
    private readonly IInventoryMovementRepository _repository;

    public ListInventoryMovementsUseCase(IInventoryMovementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> ExecuteAsync(
        ListInventoryMovementsQuery input,
        CancellationToken cancellationToken = default)
    {
        var movements = await _repository.GetAllAsync(
            input.ProductId, input.Type, input.FromUtc, input.ToUtc, cancellationToken);

        return movements.Select(ToResponse).ToList();
    }

    private static InventoryMovementResponse ToResponse(InventoryMovement movement) =>
        new(movement.Id, movement.ProductId, movement.Type, movement.Quantity,
            movement.StockBefore, movement.StockAfter, movement.Reason,
            movement.ReferenceType, movement.ReferenceId, movement.CreatedByUserId,
            movement.CreatedAtUtc);
}
