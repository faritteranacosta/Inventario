using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Tenants.Mappers;
using Unimag.Inventario.Api.Tenants.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Tenants.Commands.CreateTenant;
using Unimag.Inventario.Application.Tenants.Commands.DeleteTenant;
using Unimag.Inventario.Application.Tenants.Commands.UpdateTenantBranding;
using Unimag.Inventario.Application.Tenants.Dtos;
using Unimag.Inventario.Application.Tenants.Queries.GetTenantById;
using Unimag.Inventario.Application.Tenants.Queries.ListTenants;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Api.Tenants;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TenantsController : ControllerBase
{
    private readonly IUseCase<CreateTenantCommand, TenantResponse> _create;
    private readonly IUseCase<DeleteTenantCommand, bool> _delete;
    private readonly IUseCase<GetTenantByIdQuery, TenantResponse> _getById;
    private readonly IUseCase<ListTenantsQuery, IReadOnlyList<TenantResponse>> _list;
    private readonly IUseCase<UpdateTenantBrandingCommand, TenantResponse> _branding;
    private readonly ICurrentTenant _currentTenant;

    public TenantsController(
        IUseCase<CreateTenantCommand, TenantResponse> create,
        IUseCase<DeleteTenantCommand, bool> delete,
        IUseCase<GetTenantByIdQuery, TenantResponse> getById,
        IUseCase<ListTenantsQuery, IReadOnlyList<TenantResponse>> list,
        IUseCase<UpdateTenantBrandingCommand, TenantResponse> branding,
        ICurrentTenant currentTenant)
    {
        _create = create; _delete = delete; _getById = getById; _list = list;
        _branding = branding; _currentTenant = currentTenant;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TenantResponse>> Create([FromBody] CreateTenantRequest request, CancellationToken ct)
    {
        var result = await _create.ExecuteAsync(TenantMapper.ToCreateCommand(request), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize(Roles = "PlatformAdmin")]
    public async Task<ActionResult<IReadOnlyList<TenantResponse>>> List(CancellationToken ct)
    {
        var result = await _list.ExecuteAsync(new ListTenantsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "PlatformAdmin")]
    public async Task<ActionResult<TenantResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _getById.ExecuteAsync(new GetTenantByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPut("branding")]
    public async Task<ActionResult<TenantResponse>> UpdateBranding([FromBody] UpdateTenantBrandingRequest request, CancellationToken ct)
    {
        var result = await _branding.ExecuteAsync(new UpdateTenantBrandingCommand(request.DisplayName, request.LogoUrl), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "PlatformAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _delete.ExecuteAsync(new DeleteTenantCommand(id), ct);
        return NoContent();
    }
}