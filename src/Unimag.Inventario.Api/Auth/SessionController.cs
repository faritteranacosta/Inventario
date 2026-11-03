using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Platform.Requests;
using Unimag.Inventario.Application.Platform;
using Unimag.Inventario.Application.Platform.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;

namespace Unimag.Inventario.Api.Auth;

[ApiController]
[Authorize]
[Route("api/auth")]
public sealed class SessionController : ControllerBase
{
    private readonly IPlatformManagementService _platform;

    public SessionController(IPlatformManagementService platform)
    {
        _platform = platform;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserSessionResponse>> Me(CancellationToken ct)
    {
        var userId = GetUserId();
        return Ok(await _platform.GetCurrentUserAsync(userId, ct));
    }

    [HttpPost("context")]
    public async Task<ActionResult<object>> SelectContext(
        [FromBody] SelectTenantContextRequest request, CancellationToken ct)
    {
        var token = await _platform.SelectTenantContextAsync(
            GetUserId(), new SelectTenantContextCommand(request.TenantId), ct);
        return Ok(new { token, activeTenantId = request.TenantId });
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("The authenticated user id is invalid.");
    }
}
