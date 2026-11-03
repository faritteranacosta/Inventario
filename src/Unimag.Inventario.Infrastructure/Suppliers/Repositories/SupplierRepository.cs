using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Suppliers;
using Unimag.Inventario.Domain.Suppliers.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Suppliers.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public SupplierRepository(AppDbContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Suppliers.FirstOrDefaultAsync(
            s => s.Id == id && s.TenantId == _currentTenant.TenantId,
            cancellationToken);

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Suppliers.AsNoTracking()
            .Where(s => s.TenantId == _currentTenant.TenantId)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default) =>
        _dbContext.Suppliers.AddAsync(supplier, cancellationToken).AsTask();

    public Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _dbContext.Suppliers.Update(supplier);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
