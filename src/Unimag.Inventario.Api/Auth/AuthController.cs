using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Auth.Requests;
using Unimag.Inventario.Application.Auth.Commands.Login;
using Unimag.Inventario.Application.Auth.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Commands.RegisterUser;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Api.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IUseCase<LoginCommand, LoginResponse> _login;
    private readonly IUseCase<RegisterUserCommand, UserResponse> _register;

    public AuthController(
        IUseCase<LoginCommand, LoginResponse> login,
        IUseCase<RegisterUserCommand, UserResponse> register)
    {
        _login = login;
        _register = register;
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
}