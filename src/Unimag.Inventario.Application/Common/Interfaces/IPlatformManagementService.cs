using Unimag.Inventario.Application.Platform.Dtos;
using Unimag.Inventario.Application.Users.Dtos;

namespace Unimag.Inventario.Application.Common.Interfaces;

public interface IPlatformManagementService
{
    Task<IReadOnlyList<PlatformTenantResponse>> ListTenantsAsync(CancellationToken cancellationToken);
    Task<PlatformTenantDetailsResponse> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<PlatformTenantResponse> CreateTenantAsync(CreatePlatformTenantCommand command, CancellationToken cancellationToken);
    Task<PlatformTenantResponse> UpdateTenantStatusAsync(UpdateTenantStatusCommand command, CancellationToken cancellationToken);
    Task<IReadOnlyList<MembershipResponse>> ListMembersAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<MembershipResponse> CreateMemberAsync(CreateMembershipCommand command, CancellationToken cancellationToken);
    Task<MembershipResponse> UpdateMemberAsync(UpdateMembershipCommand command, CancellationToken cancellationToken);
    Task RevokeMemberAsync(Guid actorUserId, Guid tenantId, Guid userId, CancellationToken cancellationToken);
    Task<UserSessionResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<string> SelectTenantContextAsync(Guid userId, SelectTenantContextCommand command, CancellationToken cancellationToken);
    Task<UserResponse> BootstrapPlatformAdminAsync(BootstrapPlatformAdminCommand command, CancellationToken cancellationToken);
    Task<PlatformTenantResponse> UpdateTenantAsync(Guid actorUserId, UpdateTenantCommand command, CancellationToken cancellationToken);
    Task DeleteTenantAsync(Guid actorUserId, Guid tenantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AuditEventResponse>> ListAuditAsync(CancellationToken cancellationToken);
    Task<UserResponse> UpdateProfileAsync(UpdateProfileCommand command, CancellationToken cancellationToken);
    Task ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken);
}
