using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Common.Tenancy;
using Unimag.Inventario.Api.Purchases.Mappers;
using Unimag.Inventario.Api.Purchases.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Purchases.Commands.ChangePurchaseStatus;
using Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;
using Unimag.Inventario.Application.Purchases.Dtos;
using Unimag.Inventario.Application.Purchases.Queries.GetPurchaseById;
using Unimag.Inventario.Application.Purchases.Queries.ListPurchases;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Application.Common.Dtos;

namespace Unimag.Inventario.Api.Purchases;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PurchasesController : ControllerBase
{
    private readonly IUseCase<CreatePurchaseCommand, PurchaseResponse> _create;
    private readonly IUseCase<ChangePurchaseStatusCommand, PurchaseResponse> _changeStatus;
    private readonly IUseCase<GetPurchaseByIdQuery, PurchaseResponse> _getById;
    private readonly IUseCase<ListPurchasesQuery, PagedResponse<PurchaseResponse>> _list;
    private readonly ICurrentTenant _currentTenant;

    public PurchasesController(
        IUseCase<CreatePurchaseCommand, PurchaseResponse> create,
        IUseCase<ChangePurchaseStatusCommand, PurchaseResponse> changeStatus,
        IUseCase<GetPurchaseByIdQuery, PurchaseResponse> getById,
        IUseCase<ListPurchasesQuery, PagedResponse<PurchaseResponse>> list,
        ICurrentTenant currentTenant)
    {
        _create = create;
        _changeStatus = changeStatus;
        _getById = getById;
        _list = list;
        _currentTenant = currentTenant;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<ActionResult<PurchaseResponse>> Create(
        CreatePurchaseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _create.ExecuteAsync(
            PurchaseMapper.ToCreateCommand(_currentTenant.TenantId, GetUserId(), request),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<PurchaseResponse>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _list.ExecuteAsync(new ListPurchasesQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PurchaseResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(
            PurchaseMapper.ToGetQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<ActionResult<PurchaseResponse>> ChangeStatus(
        Guid id,
        ChangePurchaseStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _changeStatus.ExecuteAsync(
            PurchaseMapper.ToChangeStatusCommand(id, GetUserId(), request),
            cancellationToken);
        return Ok(result);
    }

    private Guid GetUserId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : Guid.Empty;
}
