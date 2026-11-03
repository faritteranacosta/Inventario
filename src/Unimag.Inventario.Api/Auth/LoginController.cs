using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Auth.Requests;
using Unimag.Inventario.Application.Auth.Commands.Login;
using Unimag.Inventario.Application.Auth.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;

namespace Unimag.Inventario.Api.Auth;

[ApiController]
[Route("api/auth")]
public sealed class LoginController : ControllerBase
{
    private readonly IUseCase<LoginCommand, LoginResponse> _login;

    public LoginController(IUseCase<LoginCommand, LoginResponse> login)
    {
        _login = login;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _login.ExecuteAsync(new LoginCommand(request.Email, request.Password), ct);
        return Ok(result);
    }
}
