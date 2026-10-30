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
    private readonly ITenantMembershipRepository _membershipRepo;

    public LoginUseCase(
        IUserRepository userRepo,
        ITenantRepository tenantRepo,
        IPasswordHasher hasher,
        ITokenService tokenService,
        ITenantMembershipRepository membershipRepo)
    {
        _userRepo = userRepo;
        _tenantRepo = tenantRepo;
        _hasher = hasher;
        _tokenService = tokenService;
        _membershipRepo = membershipRepo;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginCommand input, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByEmailAsync(input.Email, ct) ?? throw new InvalidCredentialsException();
        if (!_hasher.Verify(input.Password, user.PasswordHash)) throw new InvalidCredentialsException();
        var tenantId = user.TenantId;
        if (tenantId == Guid.Empty)
        {
            var membership = (await _membershipRepo.GetByUserIdAsync(user.Id, ct))
                .FirstOrDefault(m => m.Status == Domain.Users.MembershipStatus.Active);
            tenantId = membership?.TenantId ?? Guid.Empty;
        }

        var tenant = tenantId == Guid.Empty
            ? null
            : await _tenantRepo.GetByIdAsync(tenantId, ct);
        if (tenantId != Guid.Empty && tenant is null)
            throw new TenantNotFoundException(tenantId);
        var token = _tokenService.GenerateToken(user, tenant);
        var userResponse = new UserResponse(user.Id, user.TenantId, user.Email, user.Role.ToString(), user.CreatedAtUtc);
        return new LoginResponse(token, userResponse);
    }
}