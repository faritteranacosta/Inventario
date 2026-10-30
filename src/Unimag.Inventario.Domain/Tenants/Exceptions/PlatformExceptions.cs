using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Tenants.Exceptions;

public sealed class InvalidTenantAdministratorException : DomainException
{
    public InvalidTenantAdministratorException() : base("An administrator email and password must be provided together.") { }
}

public sealed class TenantAccessDeniedException : DomainException
{
    public TenantAccessDeniedException(Guid tenantId) : base($"The current user has no active access to tenant '{tenantId}'.") { }
}

public sealed class TenantSuspendedException : DomainException
{
    public TenantSuspendedException(Guid tenantId) : base($"Tenant '{tenantId}' is suspended.") { }
}

public sealed class InvalidTenantStatusException : DomainException
{
    public InvalidTenantStatusException(string value) : base($"Tenant status '{value}' is invalid.") { }
}

public sealed class InvalidMembershipStatusException : DomainException
{
    public InvalidMembershipStatusException(string value) : base($"Membership status '{value}' is invalid.") { }
}
