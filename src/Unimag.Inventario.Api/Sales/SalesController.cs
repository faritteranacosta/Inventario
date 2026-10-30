using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Sales.Mappers;
using Unimag.Inventario.Api.Sales.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Sales.Commands.CreateSale;
using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Sales.Queries.GetSaleById;
using Unimag.Inventario.Application.Sales.Queries.ListSales;

namespace Unimag.Inventario.Api.Sales;

[ApiController]
[Route("api/[controller]")]
public sealed class SalesController : ControllerBase
{
    private readonly IUseCase<CreateSaleCommand, SaleResponse> _create;
    private readonly IUseCase<GetSaleByIdQuery, SaleResponse> _getById;
    private readonly IUseCase<ListSalesQuery, IReadOnlyList<SaleResponse>> _list;

    public SalesController(
        IUseCase<CreateSaleCommand, SaleResponse> create,
        IUseCase<GetSaleByIdQuery, SaleResponse> getById,
        IUseCase<ListSalesQuery, IReadOnlyList<SaleResponse>> list)
    {
        _create = create;
        _getById = getById;
        _list = list;
    }

    [HttpPost]
    public async Task<ActionResult<SaleResponse>> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var result = await _create.ExecuteAsync(SaleMapper.ToCreateCommand(request), cancellationToken);
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
}