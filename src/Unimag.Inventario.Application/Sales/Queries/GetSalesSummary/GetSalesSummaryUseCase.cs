using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Domain.Sales.Contracts;

namespace Unimag.Inventario.Application.Sales.Queries.GetSalesSummary;

public sealed class GetSalesSummaryUseCase : IUseCase<GetSalesSummaryQuery, SalesSummaryResponse>
{
    private readonly ISaleRepository _saleRepo;

    public GetSalesSummaryUseCase(ISaleRepository saleRepo) => _saleRepo = saleRepo;

    public async Task<SalesSummaryResponse> ExecuteAsync(GetSalesSummaryQuery input, CancellationToken cancellationToken = default)
    {
        var sales = await _saleRepo.GetByDateRangeAsync(input.FromUtc, input.ToUtc, cancellationToken);

        var groups = input.GroupBy.ToLowerInvariant() == "month"
            ? sales.GroupBy(s => new DateTime(s.SaleDateUtc.Year, s.SaleDateUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc))
            : sales.GroupBy(s => s.SaleDateUtc.Date);

        var items = groups
            .OrderBy(g => g.Key)
            .Select(g => new SalesSummaryItem(g.Key, g.Sum(s => s.Total), g.Count()))
            .ToList();

        return new SalesSummaryResponse(items, sales.Sum(s => s.Total), sales.Count);
    }
}