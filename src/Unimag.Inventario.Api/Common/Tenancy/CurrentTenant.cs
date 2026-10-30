using Microsoft.AspNetCore.Http;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Api.Common.Tenancy;

public sealed class CurrentTenant : ICurrentTenant
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentTenant(IHttpContextAccessor accessor) => _accessor = accessor;

    public Guid TenantId
    {
        get
        {
            var claim = _accessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}