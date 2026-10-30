using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Domain.Tenants.Exceptions;

namespace Unimag.Inventario.Application.Tenants.Commands.DeleteTenant;

public sealed class DeleteTenantUseCase : IUseCase<DeleteTenantCommand, bool>
{
    private readonly ITenantRepository _repo;
    public DeleteTenantUseCase(ITenantRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(DeleteTenantCommand input, CancellationToken ct = default)
    {
        var tenant = await _repo.GetByIdAsync(input.Id, ct) ?? throw new TenantNotFoundException(input.Id);
        await _repo.DeleteAsync(tenant, ct);
        await _repo.SaveChangesAsync(ct);
        return true;
    }
}