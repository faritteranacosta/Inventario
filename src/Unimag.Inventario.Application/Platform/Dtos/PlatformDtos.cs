using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Application.Platform.Dtos;

public sealed record PlatformTenantResponse(
    Guid Id,
    string Name,
    string Status,
    DateTime CreatedAtUtc,
    int UserCount,
    int AdminCount);

public sealed record MembershipResponse(
    Guid Id,
    Guid UserId,
    Guid TenantId,
    string Email,
    string Role,
    string Status,
    DateTime CreatedAtUtc);

public sealed record UserSessionResponse(
    Guid Id,
    string Email,
    string PlatformRole,
    Guid? ActiveTenantId,
    IReadOnlyList<MembershipResponse> Memberships);

public sealed record PlatformTenantDetailsResponse(
    PlatformTenantResponse Tenant,
    IReadOnlyList<MembershipResponse> Members);

public sealed record CreatePlatformTenantCommand(
    Guid ActorUserId,
    string Name,
    string? AdminEmail,
    string? AdminPassword);

public sealed record UpdateTenantStatusCommand(Guid ActorUserId, Guid TenantId, TenantStatus Status);

public sealed record CreateMembershipCommand(
    Guid ActorUserId,
    Guid TenantId,
    string Email,
    string Password,
    UserRole Role);

public sealed record UpdateMembershipCommand(
    Guid ActorUserId,
    Guid TenantId,
    Guid UserId,
    UserRole? Role,
    MembershipStatus? Status);

public sealed record SelectTenantContextCommand(Guid TenantId);

public sealed record BootstrapPlatformAdminCommand(string Email, string Password);
public sealed record UpdateTenantCommand(Guid TenantId, string Name);
public sealed record UpdateProfileCommand(Guid UserId, string Email);
public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
public sealed record AuditEventResponse(
    Guid Id,
    Guid ActorUserId,
    string Action,
    string ResourceType,
    Guid? ResourceId,
    DateTime CreatedAtUtc);
