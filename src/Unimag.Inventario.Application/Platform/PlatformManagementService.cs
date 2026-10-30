using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Platform.Dtos;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Domain.Tenants.Exceptions;
using Unimag.Inventario.Domain.Users;
using Unimag.Inventario.Domain.Users.Contracts;

namespace Unimag.Inventario.Application.Platform;

public sealed class PlatformManagementService : IPlatformManagementService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantMembershipRepository _membershipRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public PlatformManagementService(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        ITenantMembershipRepository membershipRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _membershipRepository = membershipRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<IReadOnlyList<PlatformTenantResponse>> ListTenantsAsync(CancellationToken ct)
    {
        var tenants = await _tenantRepository.GetAllAsync(ct);
        var result = new List<PlatformTenantResponse>(tenants.Count);
        foreach (var tenant in tenants)
            result.Add(await ToTenantResponseAsync(tenant, ct));
        return result;
    }

    public async Task<PlatformTenantDetailsResponse> GetTenantAsync(Guid tenantId, CancellationToken ct)
    {
        var tenant = await GetTenantOrThrowAsync(tenantId, ct);
        return new PlatformTenantDetailsResponse(
            await ToTenantResponseAsync(tenant, ct),
            await ToMembershipResponsesAsync(tenantId, ct));
    }

    public async Task<PlatformTenantResponse> CreateTenantAsync(CreatePlatformTenantCommand command, CancellationToken ct)
    {
        var tenant = Tenant.Create(command.Name);
        await _tenantRepository.AddAsync(tenant, ct);

        if (!string.IsNullOrWhiteSpace(command.AdminEmail))
        {
            if (string.IsNullOrWhiteSpace(command.AdminPassword))
                throw new InvalidTenantAdministratorException();

            if (await _userRepository.ExistsAsync(command.AdminEmail, ct))
                throw new UserAlreadyExistsException(command.AdminEmail);

            var admin = User.Create(Guid.Empty, command.AdminEmail, _passwordHasher.Hash(command.AdminPassword), UserRole.Admin);
            await _userRepository.AddAsync(admin, ct);
            await _membershipRepository.AddAsync(TenantMembership.Create(admin.Id, tenant.Id, UserRole.Admin), ct);
        }

        await _tenantRepository.SaveChangesAsync(ct);
        return await ToTenantResponseAsync(tenant, ct);
    }

    public async Task<PlatformTenantResponse> UpdateTenantStatusAsync(UpdateTenantStatusCommand command, CancellationToken ct)
    {
        var tenant = await GetTenantOrThrowAsync(command.TenantId, ct);
        tenant.ChangeStatus(command.Status);
        await _tenantRepository.SaveChangesAsync(ct);
        return await ToTenantResponseAsync(tenant, ct);
    }

    public async Task<IReadOnlyList<MembershipResponse>> ListMembersAsync(Guid tenantId, CancellationToken ct)
    {
        await GetTenantOrThrowAsync(tenantId, ct);
        return await ToMembershipResponsesAsync(tenantId, ct);
    }

    public async Task<MembershipResponse> CreateMemberAsync(CreateMembershipCommand command, CancellationToken ct)
    {
        await GetTenantOrThrowAsync(command.TenantId, ct);
        if (command.Role == UserRole.PlatformAdmin)
            throw new InvalidMembershipRoleException();

        var user = await _userRepository.GetByEmailAsync(command.Email, ct);
        if (user is null)
        {
            user = User.Create(Guid.Empty, command.Email, _passwordHasher.Hash(command.Password), command.Role);
            await _userRepository.AddAsync(user, ct);
        }
        else if (user.Role == UserRole.PlatformAdmin)
        {
            throw new InvalidMembershipRoleException();
        }
        else if (await _membershipRepository.GetAsync(user.Id, command.TenantId, ct) is not null)
        {
            throw new UserAlreadyExistsException(command.Email);
        }

        var membership = TenantMembership.Create(user.Id, command.TenantId, command.Role);
        await _membershipRepository.AddAsync(membership, ct);
        await _membershipRepository.SaveChangesAsync(ct);
        return await ToMembershipResponseAsync(membership, user.Email);
    }

    public async Task<MembershipResponse> UpdateMemberAsync(UpdateMembershipCommand command, CancellationToken ct)
    {
        await GetTenantOrThrowAsync(command.TenantId, ct);
        var membership = await _membershipRepository.GetAsync(command.UserId, command.TenantId, ct)
            ?? throw new UserNotFoundException(command.UserId);

        if (command.Role.HasValue)
            membership.ChangeRole(command.Role.Value);
        if (command.Status.HasValue)
            membership.ChangeStatus(command.Status.Value);

        await _membershipRepository.SaveChangesAsync(ct);
        var user = await _userRepository.GetByIdAsync(command.UserId, ct)
            ?? throw new UserNotFoundException(command.UserId);
        return await ToMembershipResponseAsync(membership, user.Email);
    }

    public async Task RevokeMemberAsync(Guid tenantId, Guid userId, CancellationToken ct)
    {
        await GetTenantOrThrowAsync(tenantId, ct);
        var membership = await _membershipRepository.GetAsync(userId, tenantId, ct)
            ?? throw new UserNotFoundException(userId);
        membership.ChangeStatus(MembershipStatus.Revoked);
        await _membershipRepository.SaveChangesAsync(ct);
    }

    public async Task<UserSessionResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct) ?? throw new UserNotFoundException(userId);
        var memberships = await ToMembershipResponsesAsync(await _membershipRepository.GetByUserIdAsync(userId, ct), ct);
        return new UserSessionResponse(user.Id, user.Email, user.Role.ToString(), null, memberships);
    }

    public async Task<string> SelectTenantContextAsync(Guid userId, SelectTenantContextCommand command, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct) ?? throw new UserNotFoundException(userId);
        var tenant = await GetTenantOrThrowAsync(command.TenantId, ct);
        if (tenant.Status == TenantStatus.Suspended)
            throw new TenantSuspendedException(command.TenantId);
        var membership = await _membershipRepository.GetAsync(userId, command.TenantId, ct);
        if (user.Role != UserRole.PlatformAdmin &&
            (membership is null || membership.Status != MembershipStatus.Active))
            throw new TenantAccessDeniedException(command.TenantId);

        return _tokenService.GenerateToken(user, tenant, membership?.Role ?? UserRole.Admin);
    }

    private async Task<Tenant> GetTenantOrThrowAsync(Guid id, CancellationToken ct)
        => await _tenantRepository.GetByIdAsync(id, ct) ?? throw new TenantNotFoundException(id);

    private async Task<PlatformTenantResponse> ToTenantResponseAsync(Tenant tenant, CancellationToken ct)
    {
        var members = await _membershipRepository.GetByTenantIdAsync(tenant.Id, ct);
        return new PlatformTenantResponse(
            tenant.Id,
            tenant.Name,
            tenant.Status.ToString(),
            tenant.CreatedAtUtc,
            members.Count(m => m.Status == MembershipStatus.Active),
            members.Count(m => m.Status == MembershipStatus.Active && m.Role == UserRole.Admin));
    }

    private async Task<IReadOnlyList<MembershipResponse>> ToMembershipResponsesAsync(Guid tenantId, CancellationToken ct)
        => await ToMembershipResponsesAsync(await _membershipRepository.GetByTenantIdAsync(tenantId, ct), ct);

    private async Task<IReadOnlyList<MembershipResponse>> ToMembershipResponsesAsync(
        IReadOnlyList<TenantMembership> memberships, CancellationToken ct)
    {
        var users = await _userRepository.GetByIdsAsync(memberships.Select(m => m.UserId).ToArray(), ct);
        var emails = users.ToDictionary(u => u.Id, u => u.Email);
        return memberships
            .Select(m => new MembershipResponse(m.Id, m.UserId, m.TenantId, emails[m.UserId],
                m.Role.ToString(), m.Status.ToString(), m.CreatedAtUtc))
            .ToList();
    }

    private static Task<MembershipResponse> ToMembershipResponseAsync(TenantMembership membership, string email)
        => Task.FromResult(new MembershipResponse(
            membership.Id, membership.UserId, membership.TenantId, email,
            membership.Role.ToString(), membership.Status.ToString(), membership.CreatedAtUtc));
}
