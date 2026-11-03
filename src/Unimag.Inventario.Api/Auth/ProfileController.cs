using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Auth.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Platform;
using Unimag.Inventario.Application.Platform.Dtos;
using Unimag.Inventario.Application.Users.Dtos;

namespace Unimag.Inventario.Api.Auth;

[ApiController]
[Authorize]
[Route("api/auth")]
public sealed class ProfileController : ControllerBase
{
    private readonly IPlatformManagementService _platform;

    public ProfileController(IPlatformManagementService platform)
    {
        _platform = platform;
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateProfile(
        [FromBody] UpdateProfileRequest request, CancellationToken ct)
        => Ok(await _platform.UpdateProfileAsync(
            new UpdateProfileCommand(GetUserId(), request.Email), ct));

    [HttpPost("me/password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await _platform.ChangePasswordAsync(
            new ChangePasswordCommand(GetUserId(), request.CurrentPassword, request.NewPassword), ct);
        return NoContent();
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
