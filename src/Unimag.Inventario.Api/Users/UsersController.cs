using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Application.Users.Queries.GetUserById;

namespace Unimag.Inventario.Api.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IUseCase<GetUserByIdQuery, UserResponse> _getById;

    public UsersController(IUseCase<GetUserByIdQuery, UserResponse> getById)
    {
        _getById = getById;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _getById.ExecuteAsync(new GetUserByIdQuery(id), ct);
        return Ok(result);
    }
}