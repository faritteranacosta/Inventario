using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Tenants.Dtos;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Domain.Tenants.Exceptions;

namespace Unimag.Inventario.Application.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdUseCase : IUseCase<GetTenantByIdQuery, TenantResponse>
{
    private readonly ITenantRepository _repo;
    public GetTenantByIdUseCase(ITenantRepository repo) => _repo = repo;

    public async Task<TenantResponse> ExecuteAsync(GetTenantByIdQuery input, CancellationToken ct = default)
    {
        var tenant = await _repo.GetByIdAsync(input.Id, ct) ?? throw new TenantNotFoundException(input.Id);
        return new TenantResponse(tenant.Id, tenant.Name, tenant.CreatedAtUtc);
    }
}