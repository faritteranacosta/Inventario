using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.InventoryMovements;
using Unimag.Inventario.Domain.InventoryMovements.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.InventoryMovements.Repositories;

public sealed class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public InventoryMovementRepository(AppDbContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.InventoryMovements.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.TenantId == _currentTenant.TenantId, cancellationToken);

    public async Task<IReadOnlyList<InventoryMovement>> GetAllAsync(
        Guid? productId,
        InventoryMovementType? type,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.InventoryMovements.AsNoTracking()
            .Where(m => m.TenantId == _currentTenant.TenantId);

        if (productId.HasValue)
        {
            query = query.Where(m => m.ProductId == productId.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(m => m.Type == type.Value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(m => m.CreatedAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(m => m.CreatedAtUtc <= toUtc.Value);
        }

        return await query.OrderByDescending(m => m.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default) =>
        _dbContext.InventoryMovements.AddAsync(movement, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
