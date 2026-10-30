using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Platform.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Platform.Dtos;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Exceptions;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Api.Platform;

[ApiController]
[Route("api/platform")]
[Authorize(Roles = nameof(UserRole.PlatformAdmin))]
public sealed class PlatformController : ControllerBase
{
    private readonly IPlatformManagementService _service;

    public PlatformController(IPlatformManagementService service) => _service = service;

    [HttpGet("tenants")]
    public async Task<ActionResult<IReadOnlyList<PlatformTenantResponse>>> ListTenants(CancellationToken ct)
        => Ok(await _service.ListTenantsAsync(ct));

    [HttpPost("bootstrap")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Bootstrap(
        BootstrapPlatformAdminRequest request, CancellationToken ct)
        => Ok(await _service.BootstrapPlatformAdminAsync(
            new BootstrapPlatformAdminCommand(request.Email, request.Password), ct));

    [HttpGet("audit")]
    public async Task<ActionResult<IReadOnlyList<AuditEventResponse>>> Audit(CancellationToken ct)
        => Ok(await _service.ListAuditAsync(ct));

    [HttpPost("tenants")]
    public async Task<ActionResult<PlatformTenantResponse>> CreateTenant(
        CreatePlatformTenantRequest request, CancellationToken ct)
    {
        var result = await _service.CreateTenantAsync(
            new CreatePlatformTenantCommand(GetUserId(), request.Name, request.AdminEmail, request.AdminPassword), ct);
        return CreatedAtAction(nameof(GetTenant), new { tenantId = result.Id }, result);
    }

    [HttpGet("tenants/{tenantId:guid}")]
    public async Task<ActionResult<PlatformTenantDetailsResponse>> GetTenant(Guid tenantId, CancellationToken ct)
        => Ok(await _service.GetTenantAsync(tenantId, ct));

    [HttpPut("tenants/{tenantId:guid}")]
    public async Task<ActionResult<PlatformTenantResponse>> RenameTenant(
        Guid tenantId, RenameTenantRequest request, CancellationToken ct)
        => Ok(await _service.UpdateTenantAsync(GetUserId(),
            new UpdateTenantCommand(tenantId, request.Name), ct));

    [HttpDelete("tenants/{tenantId:guid}")]
    public async Task<IActionResult> DeleteTenant(Guid tenantId, CancellationToken ct)
    {
        await _service.DeleteTenantAsync(GetUserId(), tenantId, ct);
        return NoContent();
    }

    [HttpPatch("tenants/{tenantId:guid}/status")]
    public async Task<ActionResult<PlatformTenantResponse>> UpdateTenantStatus(
        Guid tenantId, UpdateTenantStatusRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<TenantStatus>(request.Status, true, out var status))
            throw new InvalidTenantStatusException(request.Status);

        return Ok(await _service.UpdateTenantStatusAsync(new UpdateTenantStatusCommand(GetUserId(), tenantId, status), ct));
    }

    [HttpGet("tenants/{tenantId:guid}/members")]
    public async Task<ActionResult<IReadOnlyList<MembershipResponse>>> ListMembers(Guid tenantId, CancellationToken ct)
        => Ok(await _service.ListMembersAsync(tenantId, ct));

    [HttpPost("tenants/{tenantId:guid}/members")]
    public async Task<ActionResult<MembershipResponse>> CreateMember(
        Guid tenantId, CreateMembershipRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            throw new InvalidMembershipRoleException();

        var result = await _service.CreateMemberAsync(
            new CreateMembershipCommand(GetUserId(), tenantId, request.Email, request.Password, role), ct);
        return CreatedAtAction(nameof(ListMembers), new { tenantId }, result);
    }

    [HttpPatch("tenants/{tenantId:guid}/members/{userId:guid}")]
    public async Task<ActionResult<MembershipResponse>> UpdateMember(
        Guid tenantId, Guid userId, UpdateMembershipRequest request, CancellationToken ct)
    {
        UserRole? role = null;
        MembershipStatus? status = null;
        if (request.Role is not null)
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out var parsedRole))
                throw new InvalidMembershipRoleException();
            role = parsedRole;
        }
        if (request.Status is not null)
        {
            if (!Enum.TryParse<MembershipStatus>(request.Status, true, out var parsedStatus))
                throw new InvalidMembershipStatusException(request.Status);
            status = parsedStatus;
        }

        return Ok(await _service.UpdateMemberAsync(
            new UpdateMembershipCommand(GetUserId(), tenantId, userId, role, status), ct));
    }

    [HttpDelete("tenants/{tenantId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RevokeMember(Guid tenantId, Guid userId, CancellationToken ct)
    {
        await _service.RevokeMemberAsync(GetUserId(), tenantId, userId, ct);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var value = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("The authenticated user id is invalid.");
    }

}
