using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Sales.Mappers;
using Unimag.Inventario.Api.Sales.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Sales.Commands.CreateSale;
using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Sales.Queries.GetSaleById;
using Unimag.Inventario.Application.Sales.Queries.GetSalesSummary;
using Unimag.Inventario.Application.Sales.Queries.ListSales;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Api.Sales;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SalesController : ControllerBase
{
    private readonly IUseCase<CreateSaleCommand, SaleResponse> _create;
    private readonly IUseCase<GetSaleByIdQuery, SaleResponse> _getById;
    private readonly IUseCase<ListSalesQuery, IReadOnlyList<SaleResponse>> _list;
    private readonly IUseCase<GetSalesSummaryQuery, SalesSummaryResponse> _summary;
    private readonly ICurrentTenant _currentTenant;

    public SalesController(
        IUseCase<CreateSaleCommand, SaleResponse> create,
        IUseCase<GetSaleByIdQuery, SaleResponse> getById,
        IUseCase<ListSalesQuery, IReadOnlyList<SaleResponse>> list,
        IUseCase<GetSalesSummaryQuery, SalesSummaryResponse> summary,
        ICurrentTenant currentTenant)
    {
        _create = create;
        _getById = getById;
        _list = list;
        _summary = summary;
        _currentTenant = currentTenant;
    }

    [HttpPost]
    public async Task<ActionResult<SaleResponse>> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var result = await _create.ExecuteAsync(SaleMapper.ToCreateCommand(_currentTenant.TenantId, request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SaleResponse>>> List(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var result = await _list.ExecuteAsync(SaleMapper.ToListQuery(from, to), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SaleResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(SaleMapper.ToGetQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<SalesSummaryResponse>> Summary(
        [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string groupBy = "day",
        CancellationToken cancellationToken = default)
    {
        var result = await _summary.ExecuteAsync(new GetSalesSummaryQuery(from, to, groupBy), cancellationToken);
        return Ok(result);
    }
}