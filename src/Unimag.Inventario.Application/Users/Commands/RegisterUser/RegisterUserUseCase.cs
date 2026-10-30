using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Users;
using Unimag.Inventario.Domain.Users.Contracts;

namespace Unimag.Inventario.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserUseCase : IUseCase<RegisterUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITenantMembershipRepository _membershipRepo;
    public RegisterUserUseCase(
        IUserRepository userRepo,
        IPasswordHasher hasher,
        ITenantMembershipRepository membershipRepo)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _membershipRepo = membershipRepo;
    }

    public async Task<UserResponse> ExecuteAsync(RegisterUserCommand input, CancellationToken ct = default)
    {
        if (await _userRepo.ExistsAsync(input.Email, ct))
            throw new UserAlreadyExistsException(input.Email);

        var hash = _hasher.Hash(input.Password);
        var user = User.Create(input.TenantId, input.Email, hash, input.Role);
        await _userRepo.AddAsync(user, ct);
        if (input.Role != UserRole.PlatformAdmin)
            await _membershipRepo.AddAsync(TenantMembership.Create(user.Id, input.TenantId, input.Role), ct);
        await _userRepo.SaveChangesAsync(ct);
        return new UserResponse(user.Id, user.TenantId, user.Email, user.Role.ToString(), user.CreatedAtUtc);
    }
}