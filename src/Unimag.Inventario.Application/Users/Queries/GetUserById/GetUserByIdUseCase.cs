using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Domain.Users.Contracts;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdUseCase : IUseCase<GetUserByIdQuery, UserResponse>
{
    private readonly IUserRepository _repo;
    public GetUserByIdUseCase(IUserRepository repo) => _repo = repo;

    public async Task<UserResponse> ExecuteAsync(GetUserByIdQuery input, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(input.Id, ct) ?? throw new UserNotFoundException(input.Id);
        return new UserResponse(user.Id, user.TenantId, user.Email, user.Role.ToString(), user.CreatedAtUtc);
    }
}