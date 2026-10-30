using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Users;

public enum MembershipStatus
{
    Active = 0,
    Suspended = 1,
    Revoked = 2
}

public sealed class TenantMembership : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public UserRole Role { get; private set; }
    public MembershipStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private TenantMembership() { }

    private TenantMembership(Guid id, Guid userId, Guid tenantId, UserRole role)
    {
        Id = id;
        UserId = userId;
        TenantId = tenantId;
        Role = role;
        Status = MembershipStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static TenantMembership Create(Guid userId, Guid tenantId, UserRole role)
    {
        if (role == UserRole.PlatformAdmin)
            throw new InvalidMembershipRoleException();

        return new TenantMembership(Guid.NewGuid(), userId, tenantId, role);
    }

    public void ChangeRole(UserRole role)
    {
        if (role == UserRole.PlatformAdmin)
            throw new InvalidMembershipRoleException();

        Role = role;
    }

    public void ChangeStatus(MembershipStatus status) => Status = status;
}

public sealed class InvalidMembershipRoleException : DomainException
{
    public InvalidMembershipRoleException() : base("A tenant membership must use the Admin or User role.") { }
}
