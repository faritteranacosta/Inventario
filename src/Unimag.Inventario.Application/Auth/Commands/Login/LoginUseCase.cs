using Unimag.Inventario.Application.Auth.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Tenants.Contracts;
using Unimag.Inventario.Domain.Tenants.Exceptions;
using Unimag.Inventario.Domain.Users.Contracts;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Application.Auth.Commands.Login;

public sealed class LoginUseCase : IUseCase<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly ITenantRepository _tenantRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;

    public LoginUseCase(IUserRepository userRepo, ITenantRepository tenantRepo, IPasswordHasher hasher, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tenantRepo = tenantRepo;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginCommand input, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByEmailAsync(input.Email, ct) ?? throw new InvalidCredentialsException();
        if (!_hasher.Verify(input.Password, user.PasswordHash)) throw new InvalidCredentialsException();
        var tenant = await _tenantRepo.GetByIdAsync(user.TenantId, ct) ?? throw new TenantNotFoundException(user.TenantId);
        var token = _tokenService.GenerateToken(user, tenant);
        var userResponse = new UserResponse(user.Id, user.TenantId, user.Email, user.Role.ToString(), user.CreatedAtUtc);
        return new LoginResponse(token, userResponse);
    }
}