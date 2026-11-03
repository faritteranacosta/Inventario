using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Auth.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Commands.RegisterUser;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Api.Auth;

[ApiController]
[Route("api/auth")]
public sealed class RegistrationController : ControllerBase
{
    private readonly IUseCase<RegisterUserCommand, UserResponse> _register;

    public RegistrationController(IUseCase<RegisterUserCommand, UserResponse> register)
    {
        _register = register;
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
