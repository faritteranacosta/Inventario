using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.InventoryMovements.Dtos;
using Unimag.Inventario.Domain.InventoryMovements;
using Unimag.Inventario.Domain.InventoryMovements.Contracts;
using Unimag.Inventario.Application.Common.Dtos;
using Unimag.Inventario.Domain.Common.Exceptions;

namespace Unimag.Inventario.Application.InventoryMovements.Queries.ListInventoryMovements;

public sealed class ListInventoryMovementsUseCase
    : IUseCase<ListInventoryMovementsQuery, PagedResponse<InventoryMovementResponse>>
{
    private readonly IInventoryMovementRepository _repository;

    public ListInventoryMovementsUseCase(IInventoryMovementRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<InventoryMovementResponse>> ExecuteAsync(
        ListInventoryMovementsQuery input,
        CancellationToken cancellationToken = default)
    {
        if (input.Page < 1 || input.PageSize is < 1 or > 100)
        {
            throw new InvalidPaginationException();
        }

        var result = await _repository.GetPageAsync(
            input.ProductId, input.Type, input.FromUtc, input.ToUtc,
            (input.Page - 1) * input.PageSize, input.PageSize, cancellationToken);
        var items = result.Items
            .Select(ToResponse)
            .ToList();
        return new PagedResponse<InventoryMovementResponse>(
            items, input.Page, input.PageSize, result.TotalItems,
            (int)Math.Ceiling(result.TotalItems / (double)input.PageSize));
    }

    private static InventoryMovementResponse ToResponse(InventoryMovement movement) =>
        new(movement.Id, movement.ProductId, movement.Type, movement.Quantity,
            movement.StockBefore, movement.StockAfter, movement.Reason,
            movement.ReferenceType, movement.ReferenceId, movement.CreatedByUserId,
            movement.CreatedAtUtc);
}
