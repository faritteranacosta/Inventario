using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Domain.Users;
using Unimag.Inventario.Domain.Users.Contracts;

namespace Unimag.Inventario.UnitTests.Fakes;

public sealed class InMemoryTenantRepository : ITenantRepository
{
    public Dictionary<Guid, Tenant> Items { get; } = new();

    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.GetValueOrDefault(id));

    public Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Tenant>>(Items.Values.ToList());

    public Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        Items[tenant.Id] = tenant;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        Items.Remove(tenant.Id);
        return Task.CompletedTask;
    }

    public Task DeleteGraphAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        Items.Remove(tenantId);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed class InMemoryUserRepository : IUserRepository
{
    public Dictionary<Guid, User> Items { get; } = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.GetValueOrDefault(id));

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Values.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<User>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(Items.Values.Where(u => ids.Contains(u.Id)).ToList());

    public Task<bool> ExistsPlatformAdminAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Values.Any(u => u.Role == UserRole.PlatformAdmin));

    public Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Values.Any(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        Items[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed class InMemoryMembershipRepository : ITenantMembershipRepository
{
    public Dictionary<Guid, TenantMembership> Items { get; } = new();

    public Task<TenantMembership?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.GetValueOrDefault(id));

    public Task<TenantMembership?> GetAsync(
        Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Values.FirstOrDefault(m => m.UserId == userId && m.TenantId == tenantId));

    public Task<IReadOnlyList<TenantMembership>> GetByTenantIdAsync(
        Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TenantMembership>>(
            Items.Values.Where(m => m.TenantId == tenantId).ToList());

    public Task<IReadOnlyList<TenantMembership>> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TenantMembership>>(
            Items.Values.Where(m => m.UserId == userId).ToList());

    public Task AddAsync(TenantMembership membership, CancellationToken cancellationToken = default)
    {
        Items[membership.Id] = membership;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(TenantMembership membership, CancellationToken cancellationToken = default)
    {
        Items.Remove(membership.Id);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed class InMemoryPlatformAuditRepository : IPlatformAuditRepository
{
    public List<PlatformAuditEvent> Items { get; } = [];

    public Task AddAsync(PlatformAuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        Items.Add(auditEvent);
        return Task.CompletedTask;
    }

    public Task<(IReadOnlyList<PlatformAuditEvent> Items, int TotalItems)> GetPageAsync(
        Guid? actorUserId, string? action, string? resourceType, DateTime? fromUtc, DateTime? toUtc,
        int skip, int take, CancellationToken cancellationToken = default)
    {
        var query = Items.AsEnumerable();
        if (actorUserId.HasValue)
            query = query.Where(e => e.ActorUserId == actorUserId.Value);
        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(e => e.Action == action);
        if (!string.IsNullOrWhiteSpace(resourceType))
            query = query.Where(e => e.ResourceType == resourceType);
        if (fromUtc.HasValue)
            query = query.Where(e => e.CreatedAtUtc >= fromUtc.Value);
        if (toUtc.HasValue)
            query = query.Where(e => e.CreatedAtUtc <= toUtc.Value);
        var ordered = query.OrderByDescending(e => e.CreatedAtUtc).ToList();
        return Task.FromResult<(IReadOnlyList<PlatformAuditEvent>, int)>(
            (ordered.Skip(skip).Take(take).ToList(), ordered.Count));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed class TestPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hash:{password}";

    public bool Verify(string password, string hash) => hash == Hash(password);
}

public sealed class TestTokenService : ITokenService
{
    public string GenerateToken(User user, Tenant? tenant, UserRole? activeRole = null)
        => $"{user.Id}:{tenant?.Id}:{activeRole ?? user.Role}";
}
