using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.InventoryMovements.Dtos;
using Unimag.Inventario.Domain.InventoryMovements;
using Unimag.Inventario.Domain.InventoryMovements.Contracts;
using Unimag.Inventario.Domain.InventoryMovements.Exceptions;

namespace Unimag.Inventario.Application.InventoryMovements.Queries.GetInventoryMovementById;

public sealed class GetInventoryMovementByIdUseCase
    : IUseCase<GetInventoryMovementByIdQuery, InventoryMovementResponse>
{
    private readonly IInventoryMovementRepository _repository;

    public GetInventoryMovementByIdUseCase(IInventoryMovementRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryMovementResponse> ExecuteAsync(
        GetInventoryMovementByIdQuery input,
        CancellationToken cancellationToken = default)
    {
        var movement = await _repository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new InventoryMovementNotFoundException(input.Id);

        return ToResponse(movement);
    }

    private static InventoryMovementResponse ToResponse(InventoryMovement movement) =>
        new(movement.Id, movement.ProductId, movement.Type, movement.Quantity,
            movement.StockBefore, movement.StockAfter, movement.Reason,
            movement.ReferenceType, movement.ReferenceId, movement.CreatedByUserId,
            movement.CreatedAtUtc);
}
