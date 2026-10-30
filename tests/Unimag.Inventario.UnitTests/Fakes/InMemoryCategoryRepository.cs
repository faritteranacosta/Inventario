using Unimag.Inventario.Domain.Categories;
using Unimag.Inventario.Domain.Categories.Contracts;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.UnitTests.Fakes;

public sealed class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly Dictionary<Guid, Category> _store = new();
    private readonly ICurrentTenant _tenant;

    public InMemoryCategoryRepository(ICurrentTenant tenant)
    {
        _tenant = tenant;
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.TryGetValue(id, out var c) && c.TenantId == _tenant.TenantId ? c : null);

    public Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Category>>(_store.Values.Where(c => c.TenantId == _tenant.TenantId).ToList());

    public Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        _store[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _store[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        _store.Remove(category.Id);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}