using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Tenants.Dtos;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Domain.Tenants.Exceptions;

namespace Unimag.Inventario.Application.Tenants.Commands.UpdateTenantBranding;

public sealed class UpdateTenantBrandingUseCase : IUseCase<UpdateTenantBrandingCommand, TenantResponse>
{
    private readonly ITenantRepository _repo;
    private readonly ICurrentTenant _currentTenant;

    public UpdateTenantBrandingUseCase(ITenantRepository repo, ICurrentTenant currentTenant)
    {
        _repo = repo;
        _currentTenant = currentTenant;
    }

    public async Task<TenantResponse> ExecuteAsync(UpdateTenantBrandingCommand input, CancellationToken ct = default)
    {
        var tenant = await _repo.GetByIdAsync(_currentTenant.TenantId, ct)
            ?? throw new TenantNotFoundException(_currentTenant.TenantId);
        tenant.SetBranding(input.DisplayName, input.LogoUrl);
        await _repo.UpdateAsync(tenant, ct);
        await _repo.SaveChangesAsync(ct);
        return new TenantResponse(tenant.Id, tenant.Name, tenant.DisplayName, tenant.LogoUrl, tenant.CreatedAtUtc);
    }
}