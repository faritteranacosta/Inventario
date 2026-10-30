using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Sales.Contracts;

namespace Unimag.Inventario.Application.Sales.Queries.ListSales;

public sealed class ListSalesUseCase : IUseCase<ListSalesQuery, IReadOnlyList<SaleResponse>>
{
    private readonly ISaleRepository _saleRepository;

    public ListSalesUseCase(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<IReadOnlyList<SaleResponse>> ExecuteAsync(ListSalesQuery input, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Sale> sales;
        if (input.FromUtc.HasValue && input.ToUtc.HasValue)
        {
            sales = await _saleRepository.GetByDateRangeAsync(input.FromUtc.Value, input.ToUtc.Value, cancellationToken);
        }
        else
        {
            sales = await _saleRepository.GetAllAsync(cancellationToken);
        }

        return sales.Select(s => new SaleResponse(s.Id, s.SaleDateUtc,
            s.Items.Select(i => new SaleItemResponse(i.ProductId, i.Quantity, i.UnitPrice, i.Total)).ToList(),
            s.Total)).ToList();
    }
}