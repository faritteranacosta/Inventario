using Unimag.Inventario.Api.Tenants.Requests;
using Unimag.Inventario.Application.Tenants.Commands.CreateTenant;

namespace Unimag.Inventario.Api.Tenants.Mappers;

public static class TenantMapper
{
    public static CreateTenantCommand ToCreateCommand(CreateTenantRequest request) => new(request.Name);
}