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

    public Task<Sale?> GetByIdempotencyKeyAsync(string key, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Values.FirstOrDefault(s =>
            s.TenantId == _tenant.TenantId && s.IdempotencyKey == key));

    public Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Sale>>(_store.Values.Where(s => s.TenantId == _tenant.TenantId).ToList());

    public Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Sale>>(_store.Values.Where(s => s.TenantId == _tenant.TenantId && s.SaleDateUtc >= fromUtc && s.SaleDateUtc <= toUtc).ToList());

    public Task<(IReadOnlyList<Sale> Items, int TotalItems)> GetPageAsync(
        DateTime? fromUtc, DateTime? toUtc, int skip, int take,
        CancellationToken cancellationToken = default)
    {
        var query = _store.Values.Where(s => s.TenantId == _tenant.TenantId);
        if (fromUtc.HasValue)
            query = query.Where(s => s.SaleDateUtc >= fromUtc.Value);
        if (toUtc.HasValue)
            query = query.Where(s => s.SaleDateUtc <= toUtc.Value);
        var ordered = query.OrderByDescending(s => s.SaleDateUtc).ThenBy(s => s.Id).ToList();
        return Task.FromResult<(IReadOnlyList<Sale>, int)>(
            (ordered.Skip(skip).Take(take).ToList(), ordered.Count));
    }

    public Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _store[sale.Id] = sale;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}