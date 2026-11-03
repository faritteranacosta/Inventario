using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Common.Tenancy;
using Unimag.Inventario.Api.Suppliers.Mappers;
using Unimag.Inventario.Api.Suppliers.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;
using Unimag.Inventario.Application.Suppliers.Commands.DeleteSupplier;
using Unimag.Inventario.Application.Suppliers.Commands.UpdateSupplier;
using Unimag.Inventario.Application.Suppliers.Dtos;
using Unimag.Inventario.Application.Suppliers.Queries.GetSupplierById;
using Unimag.Inventario.Application.Suppliers.Queries.ListSuppliers;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Api.Suppliers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SuppliersController : ControllerBase
{
    private readonly IUseCase<CreateSupplierCommand, SupplierResponse> _create;
    private readonly IUseCase<UpdateSupplierCommand, SupplierResponse> _update;
    private readonly IUseCase<DeleteSupplierCommand, bool> _delete;
    private readonly IUseCase<GetSupplierByIdQuery, SupplierResponse> _getById;
    private readonly IUseCase<ListSuppliersQuery, IReadOnlyList<SupplierResponse>> _list;
    private readonly ICurrentTenant _currentTenant;

    public SuppliersController(
        IUseCase<CreateSupplierCommand, SupplierResponse> create,
        IUseCase<UpdateSupplierCommand, SupplierResponse> update,
        IUseCase<DeleteSupplierCommand, bool> delete,
        IUseCase<GetSupplierByIdQuery, SupplierResponse> getById,
        IUseCase<ListSuppliersQuery, IReadOnlyList<SupplierResponse>> list,
        ICurrentTenant currentTenant)
    {
        _create = create;
        _update = update;
        _delete = delete;
        _getById = getById;
        _list = list;
        _currentTenant = currentTenant;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<ActionResult<SupplierResponse>> Create(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _create.ExecuteAsync(
            SupplierMapper.ToCreateCommand(_currentTenant.TenantId, request),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SupplierResponse>>> List(
        CancellationToken cancellationToken)
    {
        var result = await _list.ExecuteAsync(new ListSuppliersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SupplierResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(
            SupplierMapper.ToGetQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<ActionResult<SupplierResponse>> Update(
        Guid id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _update.ExecuteAsync(
            SupplierMapper.ToUpdateCommand(id, request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _delete.ExecuteAsync(
            SupplierMapper.ToDeleteCommand(id), cancellationToken);
        return NoContent();
    }
}
