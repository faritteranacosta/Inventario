using Unimag.Inventario.Application.Platform;
using Unimag.Inventario.Application.Platform.Dtos;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Exceptions;
using Unimag.Inventario.Domain.Users;
using Unimag.Inventario.UnitTests.Fakes;

namespace Unimag.Inventario.UnitTests.Platform;

public sealed class PlatformManagementServiceTests
{
    private readonly InMemoryTenantRepository _tenants = new();
    private readonly InMemoryUserRepository _users = new();
    private readonly InMemoryMembershipRepository _memberships = new();
    private readonly InMemoryPlatformAuditRepository _audit = new();
    private readonly PlatformManagementService _service;

    public PlatformManagementServiceTests()
    {
        _service = new PlatformManagementService(
            _tenants,
            _users,
            _memberships,
            new TestPasswordHasher(),
            new TestTokenService(),
            _audit);
    }

    [Fact]
    public async Task Bootstrap_CreatesOnlyTheFirstPlatformAdmin()
    {
        var first = await _service.BootstrapPlatformAdminAsync(
            new BootstrapPlatformAdminCommand("platform@example.com", "secret"), CancellationToken.None);

        Assert.Equal("PlatformAdmin", first.Role);
        Assert.Single(_users.Items);
        await Assert.ThrowsAsync<PlatformAdminAlreadyExistsException>(() =>
            _service.BootstrapPlatformAdminAsync(
                new BootstrapPlatformAdminCommand("second@example.com", "secret"), CancellationToken.None));
    }

    [Fact]
    public async Task CreateTenant_WithAdminCreatesMembershipAndAudit()
    {
        var actor = await AddPlatformAdminAsync();
        var tenant = await _service.CreateTenantAsync(
            new CreatePlatformTenantCommand(actor, "Store", "admin@store.com", "secret"),
            CancellationToken.None);

        Assert.Equal("Store", tenant.Name);
        Assert.Equal("Active", tenant.Status);
        Assert.Equal(1, tenant.AdminCount);
        Assert.Single(_memberships.Items);
        Assert.Contains(_audit.Items, e => e.Action == "TenantCreated");
    }

    [Fact]
    public async Task TenantQueriesReturnTenantDetailsAndMembers()
    {
        var actor = await AddPlatformAdminAsync();
        var created = await _service.CreateTenantAsync(
            new CreatePlatformTenantCommand(actor, "Store", "admin@store.com", "secret"),
            CancellationToken.None);

        var tenants = await _service.ListTenantsAsync(CancellationToken.None);
        var details = await _service.GetTenantAsync(created.Id, CancellationToken.None);
        var members = await _service.ListMembersAsync(created.Id, CancellationToken.None);

        Assert.Single(tenants);
        Assert.Equal(created.Id, details.Tenant.Id);
        Assert.Single(details.Members);
        Assert.Single(members);
    }

    [Fact]
    public async Task CreateTenant_RequiresAdminPasswordWhenEmailProvided()
    {
        await Assert.ThrowsAsync<InvalidTenantAdministratorException>(() =>
            _service.CreateTenantAsync(
                new CreatePlatformTenantCommand(Guid.NewGuid(), "Store", "admin@store.com", null),
                CancellationToken.None));
    }

    [Fact]
    public async Task TenantCanBeRenamedSuspendedAndDeleted()
    {
        var actor = await AddPlatformAdminAsync();
        var created = await _service.CreateTenantAsync(
            new CreatePlatformTenantCommand(actor, "Old", null, null), CancellationToken.None);

        var renamed = await _service.UpdateTenantAsync(
            actor, new UpdateTenantCommand(created.Id, "New"), CancellationToken.None);
        Assert.Equal("New", renamed.Name);

        var suspended = await _service.UpdateTenantStatusAsync(
            new UpdateTenantStatusCommand(actor, created.Id, TenantStatus.Suspended), CancellationToken.None);
        Assert.Equal("Suspended", suspended.Status);

        await _service.DeleteTenantAsync(actor, created.Id, CancellationToken.None);
        Assert.DoesNotContain(created.Id, _tenants.Items.Keys);
        Assert.Contains(_audit.Items, e => e.Action == "TenantDeleted");
    }

    [Fact]
    public async Task CreateAndUpdateMembership_ChangesRoleAndStatus()
    {
        var actor = await AddPlatformAdminAsync();
        var tenant = await _service.CreateTenantAsync(
            new CreatePlatformTenantCommand(actor, "Store", null, null), CancellationToken.None);

        var member = await _service.CreateMemberAsync(
            new CreateMembershipCommand(actor, tenant.Id, "user@store.com", "secret", UserRole.User),
            CancellationToken.None);
        var updated = await _service.UpdateMemberAsync(
            new UpdateMembershipCommand(actor, tenant.Id, member.UserId, UserRole.Admin, MembershipStatus.Suspended),
            CancellationToken.None);

        Assert.Equal("Admin", updated.Role);
        Assert.Equal("Suspended", updated.Status);
        Assert.Contains(_audit.Items, e => e.Action == "MembershipUpdated");
    }

    [Fact]
    public async Task CreateMembershipRejectsDuplicateMembership()
    {
        var actor = await AddPlatformAdminAsync();
        var tenant = await _service.CreateTenantAsync(
            new CreatePlatformTenantCommand(actor, "Store", null, null), CancellationToken.None);
        var command = new CreateMembershipCommand(actor, tenant.Id, "user@store.com", "secret", UserRole.User);
        await _service.CreateMemberAsync(command, CancellationToken.None);

        await Assert.ThrowsAsync<UserAlreadyExistsException>(() =>
            _service.CreateMemberAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task RevokeMembershipMarksItRevoked()
    {
        var actor = await AddPlatformAdminAsync();
        var tenant = await _service.CreateTenantAsync(
            new CreatePlatformTenantCommand(actor, "Store", null, null), CancellationToken.None);
        var member = await _service.CreateMemberAsync(
            new CreateMembershipCommand(actor, tenant.Id, "user@store.com", "secret", UserRole.User),
            CancellationToken.None);

        await _service.RevokeMemberAsync(actor, tenant.Id, member.UserId, CancellationToken.None);

        Assert.Equal(MembershipStatus.Revoked, _memberships.Items[member.Id].Status);
        Assert.Contains(_audit.Items, e => e.Action == "MembershipRevoked");
    }

    [Fact]
    public async Task SelectContextRejectsSuspendedTenantAndInactiveMembership()
    {
        var tenant = Tenant.Create("Store");
        await _tenants.AddAsync(tenant);
        var user = User.Create(Guid.Empty, "user@store.com", "hash:secret");
        await _users.AddAsync(user);

        await Assert.ThrowsAsync<TenantAccessDeniedException>(() =>
            _service.SelectTenantContextAsync(user.Id,
                new SelectTenantContextCommand(tenant.Id), CancellationToken.None));

        var membership = TenantMembership.Create(user.Id, tenant.Id, UserRole.User);
        membership.ChangeStatus(MembershipStatus.Suspended);
        await _memberships.AddAsync(membership);

        await Assert.ThrowsAsync<TenantAccessDeniedException>(() =>
            _service.SelectTenantContextAsync(user.Id,
                new SelectTenantContextCommand(tenant.Id), CancellationToken.None));

        tenant.ChangeStatus(TenantStatus.Suspended);
        membership.ChangeStatus(MembershipStatus.Active);
        await Assert.ThrowsAsync<TenantSuspendedException>(() =>
            _service.SelectTenantContextAsync(user.Id,
                new SelectTenantContextCommand(tenant.Id), CancellationToken.None));
    }

    [Fact]
    public async Task SelectContextReturnsTokenForActiveMembership()
    {
        var tenant = Tenant.Create("Store");
        var user = User.Create(Guid.Empty, "user@store.com", "hash:secret");
        var membership = TenantMembership.Create(user.Id, tenant.Id, UserRole.User);
        await _tenants.AddAsync(tenant);
        await _users.AddAsync(user);
        await _memberships.AddAsync(membership);

        var token = await _service.SelectTenantContextAsync(user.Id,
            new SelectTenantContextCommand(tenant.Id), CancellationToken.None);

        Assert.Contains(tenant.Id.ToString(), token);
        Assert.EndsWith("User", token);
    }

    [Fact]
    public async Task ProfileAndPasswordCanBeUpdated()
    {
        var user = User.Create(Guid.Empty, "old@example.com", "hash:old");
        await _users.AddAsync(user);

        var profile = await _service.UpdateProfileAsync(
            new UpdateProfileCommand(user.Id, "new@example.com"), CancellationToken.None);
        Assert.Equal("new@example.com", profile.Email);

        await _service.ChangePasswordAsync(
            new ChangePasswordCommand(user.Id, "old", "new"), CancellationToken.None);
        Assert.Equal("hash:new", _users.Items[user.Id].PasswordHash);
    }

    [Fact]
    public async Task ChangePasswordRejectsWrongCurrentPassword()
    {
        var user = User.Create(Guid.Empty, "user@example.com", "hash:old");
        await _users.AddAsync(user);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _service.ChangePasswordAsync(
                new ChangePasswordCommand(user.Id, "wrong", "new"), CancellationToken.None));
    }

    [Fact]
    public async Task CurrentUserIncludesMembershipsAndAuditCanBeListed()
    {
        var user = User.Create(Guid.Empty, "user@example.com", "hash:secret");
        var tenant = Tenant.Create("Store");
        await _users.AddAsync(user);
        await _tenants.AddAsync(tenant);
        await _memberships.AddAsync(TenantMembership.Create(user.Id, tenant.Id, UserRole.User));

        var session = await _service.GetCurrentUserAsync(user.Id, CancellationToken.None);
        var audit = await _service.ListAuditAsync(CancellationToken.None);

        Assert.Single(session.Memberships);
        Assert.Empty(audit);
    }

    private async Task<Guid> AddPlatformAdminAsync()
    {
        var result = await _service.BootstrapPlatformAdminAsync(
            new BootstrapPlatformAdminCommand("platform@example.com", "secret"), CancellationToken.None);
        return result.Id;
    }
}
