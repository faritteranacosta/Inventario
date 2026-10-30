using Unimag.Inventario.Application.Platform.Dtos;

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
    Task RevokeMemberAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
    Task<UserSessionResponse> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<string> SelectTenantContextAsync(Guid userId, SelectTenantContextCommand command, CancellationToken cancellationToken);
}
