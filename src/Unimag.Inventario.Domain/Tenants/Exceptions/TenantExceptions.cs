using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Tenants.Exceptions;

public sealed class TenantNameRequiredException : DomainException
{
    public TenantNameRequiredException() : base("Tenant name cannot be empty.") { }
}

public sealed class TenantNotFoundException : DomainException
{
    public TenantNotFoundException(Guid id) : base($"Tenant '{id}' was not found.") { }
}