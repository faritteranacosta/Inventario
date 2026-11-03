using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Common.Tenancy;
using Unimag.Inventario.Api.InventoryMovements.Mappers;
using Unimag.Inventario.Api.InventoryMovements.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.InventoryMovements.Commands.CreateInventoryMovement;
using Unimag.Inventario.Application.InventoryMovements.Dtos;
using Unimag.Inventario.Application.InventoryMovements.Queries.GetInventoryMovementById;
using Unimag.Inventario.Application.InventoryMovements.Queries.ListInventoryMovements;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.InventoryMovements;
using Unimag.Inventario.Application.Common.Dtos;

namespace Unimag.Inventario.Api.InventoryMovements;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InventoryMovementsController : ControllerBase
{
    private readonly IUseCase<CreateInventoryMovementCommand, InventoryMovementResponse> _create;
    private readonly IUseCase<GetInventoryMovementByIdQuery, InventoryMovementResponse> _getById;
    private readonly IUseCase<ListInventoryMovementsQuery, PagedResponse<InventoryMovementResponse>> _list;
    private readonly ICurrentTenant _currentTenant;

    public InventoryMovementsController(
        IUseCase<CreateInventoryMovementCommand, InventoryMovementResponse> create,
        IUseCase<GetInventoryMovementByIdQuery, InventoryMovementResponse> getById,
        IUseCase<ListInventoryMovementsQuery, PagedResponse<InventoryMovementResponse>> list,
        ICurrentTenant currentTenant)
    {
        _create = create;
        _getById = getById;
        _list = list;
        _currentTenant = currentTenant;
    }

    [HttpPost]
    public async Task<ActionResult<InventoryMovementResponse>> Create(
        CreateInventoryMovementRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _create.ExecuteAsync(
            InventoryMovementMapper.ToCreateCommand(
                _currentTenant.TenantId,
                GetUserId(),
                request),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<InventoryMovementResponse>>> List(
        [FromQuery] Guid? productId,
        [FromQuery] InventoryMovementType? type,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _list.ExecuteAsync(
            InventoryMovementMapper.ToListQuery(productId, type, from, to, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InventoryMovementResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(
            InventoryMovementMapper.ToGetQuery(id),
            cancellationToken);
        return Ok(result);
    }

    private Guid GetUserId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : Guid.Empty;
}
