using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Auth.Requests;
using Unimag.Inventario.Application.Auth.Commands.Login;
using Unimag.Inventario.Application.Auth.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Commands.RegisterUser;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Users;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Unimag.Inventario.Application.Platform.Dtos;
using Unimag.Inventario.Application.Platform;
using Unimag.Inventario.Api.Platform.Requests;

namespace Unimag.Inventario.Api.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IUseCase<LoginCommand, LoginResponse> _login;
    private readonly IUseCase<RegisterUserCommand, UserResponse> _register;
    private readonly IPlatformManagementService _platform;

    public AuthController(
        IUseCase<LoginCommand, LoginResponse> login,
        IUseCase<RegisterUserCommand, UserResponse> register,
        IPlatformManagementService platform)
    {
        _login = login;
        _register = register;
        _platform = platform;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _login.ExecuteAsync(new LoginCommand(request.Email, request.Password), ct);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            role = UserRole.User;

        var result = await _register.ExecuteAsync(
            new RegisterUserCommand(request.TenantId, request.Email, request.Password, role), ct);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserSessionResponse>> Me(CancellationToken ct)
    {
        var userId = GetUserId();
        return Ok(await _platform.GetCurrentUserAsync(userId, ct));
    }

    [Authorize]
    [HttpPost("context")]
    public async Task<ActionResult<object>> SelectContext(
        [FromBody] SelectTenantContextRequest request, CancellationToken ct)
    {
        var token = await _platform.SelectTenantContextAsync(
            GetUserId(), new SelectTenantContextCommand(request.TenantId), ct);
        return Ok(new { token, activeTenantId = request.TenantId });
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateProfile(
        [FromBody] UpdateProfileRequest request, CancellationToken ct)
        => Ok(await _platform.UpdateProfileAsync(
            new UpdateProfileCommand(GetUserId(), request.Email), ct));

    [Authorize]
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