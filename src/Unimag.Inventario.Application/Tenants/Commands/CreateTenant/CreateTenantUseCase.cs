using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Tenants.Dtos;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Contracts;

namespace Unimag.Inventario.Application.Tenants.Commands.CreateTenant;

public sealed class CreateTenantUseCase : IUseCase<CreateTenantCommand, TenantResponse>
{
    private readonly ITenantRepository _repo;
    public CreateTenantUseCase(ITenantRepository repo) => _repo = repo;

    public async Task<TenantResponse> ExecuteAsync(CreateTenantCommand input, CancellationToken ct = default)
    {
        var tenant = Tenant.Create(input.Name);
        await _repo.AddAsync(tenant, ct);
        await _repo.SaveChangesAsync(ct);
        return new TenantResponse(tenant.Id, tenant.Name, tenant.CreatedAtUtc);
    }
}