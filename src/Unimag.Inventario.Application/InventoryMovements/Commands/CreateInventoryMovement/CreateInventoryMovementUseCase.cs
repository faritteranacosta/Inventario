using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.InventoryMovements.Dtos;
using Unimag.Inventario.Domain.InventoryMovements;
using Unimag.Inventario.Domain.InventoryMovements.Contracts;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;

namespace Unimag.Inventario.Application.InventoryMovements.Commands.CreateInventoryMovement;

public sealed class CreateInventoryMovementUseCase : IUseCase<CreateInventoryMovementCommand, InventoryMovementResponse>
{
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IProductRepository _productRepository;

    public CreateInventoryMovementUseCase(
        IInventoryMovementRepository movementRepository,
        IProductRepository productRepository)
    {
        _movementRepository = movementRepository;
        _productRepository = productRepository;
    }

    public async Task<InventoryMovementResponse> ExecuteAsync(
        CreateInventoryMovementCommand input,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(input.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(input.ProductId);

        var movement = InventoryMovement.Create(
            input.TenantId,
            product.Id,
            input.Type,
            input.Quantity,
            product.Stock,
            input.Reason,
            input.ReferenceType,
            input.ReferenceId,
            input.CreatedByUserId);

        if (input.Type == InventoryMovementType.Out)
        {
            product.DecreaseStock(input.Quantity);
        }
        else
        {
            product.IncreaseStock(input.Quantity);
        }

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _movementRepository.AddAsync(movement, cancellationToken);
        await _movementRepository.SaveChangesAsync(cancellationToken);

        return ToResponse(movement);
    }

    private static InventoryMovementResponse ToResponse(InventoryMovement movement) =>
        new(
            movement.Id,
            movement.ProductId,
            movement.Type,
            movement.Quantity,
            movement.StockBefore,
            movement.StockAfter,
            movement.Reason,
            movement.ReferenceType,
            movement.ReferenceId,
            movement.CreatedByUserId,
            movement.CreatedAtUtc);
}
