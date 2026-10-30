using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Sales.Contracts;

namespace Unimag.Inventario.UnitTests.Fakes;

public sealed class InMemorySaleRepository : ISaleRepository
{
    private readonly Dictionary<Guid, Sale> _store = new();
    private readonly ICurrentTenant _tenant;

    public InMemorySaleRepository(ICurrentTenant tenant)
    {
        _tenant = tenant;
    }

    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryGetValue(id, out var s) && s.TenantId == _tenant.TenantId ? s : null);

    public Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Sale>>(_store.Values.Where(s => s.TenantId == _tenant.TenantId).ToList());

    public Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Sale>>(_store.Values.Where(s => s.TenantId == _tenant.TenantId && s.SaleDateUtc >= fromUtc && s.SaleDateUtc <= toUtc).ToList());

    public Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _store[sale.Id] = sale;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}