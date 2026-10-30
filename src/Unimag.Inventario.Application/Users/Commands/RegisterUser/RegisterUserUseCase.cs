using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Users;
using Unimag.Inventario.Domain.Users.Contracts;

namespace Unimag.Inventario.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserUseCase : IUseCase<RegisterUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    public RegisterUserUseCase(IUserRepository userRepo, IPasswordHasher hasher)
    {
        _userRepo = userRepo;
        _hasher = hasher;
    }

    public async Task<UserResponse> ExecuteAsync(RegisterUserCommand input, CancellationToken ct = default)
    {
        if (await _userRepo.ExistsAsync(input.Email, ct))
            throw new UserAlreadyExistsException(input.Email);

        var hash = _hasher.Hash(input.Password);
        var user = User.Create(input.TenantId, input.Email, hash, input.Role);
        await _userRepo.AddAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);
        return new UserResponse(user.Id, user.TenantId, user.Email, user.Role.ToString(), user.CreatedAtUtc);
    }
}