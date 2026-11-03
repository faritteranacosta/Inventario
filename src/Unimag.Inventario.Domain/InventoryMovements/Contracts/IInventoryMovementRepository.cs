using Unimag.Inventario.Domain.InventoryMovements;

namespace Unimag.Inventario.Domain.InventoryMovements.Contracts;

public interface IInventoryMovementRepository
{
    Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryMovement>> GetAllAsync(
        Guid? productId,
        InventoryMovementType? type,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);

    Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
