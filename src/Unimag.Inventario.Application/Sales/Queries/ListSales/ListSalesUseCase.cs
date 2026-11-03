using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Sales.Contracts;
using Unimag.Inventario.Application.Common.Dtos;
using Unimag.Inventario.Domain.Common.Exceptions;

namespace Unimag.Inventario.Application.Sales.Queries.ListSales;

public sealed class ListSalesUseCase : IUseCase<ListSalesQuery, PagedResponse<SaleResponse>>
{
    private readonly ISaleRepository _saleRepository;

    public ListSalesUseCase(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<PagedResponse<SaleResponse>> ExecuteAsync(ListSalesQuery input, CancellationToken cancellationToken = default)
    {
        if (input.Page < 1 || input.PageSize is < 1 or > 100)
        {
            throw new InvalidPaginationException();
        }

        IReadOnlyList<Sale> sales;
        if (input.FromUtc.HasValue && input.ToUtc.HasValue)
        {
            sales = await _saleRepository.GetByDateRangeAsync(input.FromUtc.Value, input.ToUtc.Value, cancellationToken);
        }
        else
        {
            sales = await _saleRepository.GetAllAsync(cancellationToken);
        }

        var totalItems = sales.Count;
        var items = sales
            .OrderByDescending(s => s.SaleDateUtc)
            .ThenBy(s => s.Id)
            .Skip((input.Page - 1) * input.PageSize)
            .Take(input.PageSize)
            .Select(s => new SaleResponse(s.Id, s.TenantName, s.SaleDateUtc,
            s.Items.Select(i => new SaleItemResponse(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.Total)).ToList(),
            s.Total)).ToList();

        return new PagedResponse<SaleResponse>(
            items,
            input.Page,
            input.PageSize,
            totalItems,
            (int)Math.Ceiling(totalItems / (double)input.PageSize));
    }
}