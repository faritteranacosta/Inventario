using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Reports.Mappers;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Reports.Queries.GenerateInventoryCsv;
using Unimag.Inventario.Application.Reports.Queries.GenerateSalesCsv;

namespace Unimag.Inventario.Api.Reports;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReportsController : ControllerBase
{
    private readonly IUseCase<GenerateSalesCsvQuery, byte[]> _salesCsv;
    private readonly IUseCase<GenerateInventoryCsvQuery, byte[]> _inventoryCsv;

    public ReportsController(
        IUseCase<GenerateSalesCsvQuery, byte[]> salesCsv,
        IUseCase<GenerateInventoryCsvQuery, byte[]> inventoryCsv)
    {
        _salesCsv = salesCsv;
        _inventoryCsv = inventoryCsv;
    }

    [HttpGet("sales.csv")]
    public async Task<IActionResult> SalesCsv(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var content = await _salesCsv.ExecuteAsync(
            ReportMapper.ToSalesCsvQuery(from, to), cancellationToken);
        return File(content, "text/csv; charset=utf-8", "sales.csv");
    }

    [HttpGet("inventory.csv")]
    public async Task<IActionResult> InventoryCsv(CancellationToken cancellationToken)
    {
        var content = await _inventoryCsv.ExecuteAsync(
            new GenerateInventoryCsvQuery(), cancellationToken);
        return File(content, "text/csv; charset=utf-8", "inventory.csv");
    }
}
