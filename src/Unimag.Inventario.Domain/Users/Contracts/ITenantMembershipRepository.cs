using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Domain.Users.Contracts;

public interface ITenantMembershipRepository
{
    Task<TenantMembership?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantMembership?> GetAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantMembership>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantMembership>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(TenantMembership membership, CancellationToken cancellationToken = default);
    Task RemoveAsync(TenantMembership membership, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
