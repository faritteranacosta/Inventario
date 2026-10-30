using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Tenants.Dtos;
using Unimag.Inventario.Domain.Tenants.Contracts;

namespace Unimag.Inventario.Application.Tenants.Queries.ListTenants;

public sealed class ListTenantsUseCase : IUseCase<ListTenantsQuery, IReadOnlyList<TenantResponse>>
{
    private readonly ITenantRepository _repo;
    public ListTenantsUseCase(ITenantRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<TenantResponse>> ExecuteAsync(ListTenantsQuery input, CancellationToken ct = default)
    {
        var tenants = await _repo.GetAllAsync(ct);
        return tenants.Select(t => new TenantResponse(t.Id, t.Name, t.CreatedAtUtc)).ToList();
    }
}