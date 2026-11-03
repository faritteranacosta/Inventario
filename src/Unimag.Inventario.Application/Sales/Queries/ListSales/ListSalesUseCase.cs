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

        var result = await _saleRepository.GetPageAsync(
            input.FromUtc, input.ToUtc,
            (input.Page - 1) * input.PageSize, input.PageSize, cancellationToken);
        var items = result.Items
            .Select(s => new SaleResponse(s.Id, s.TenantName, s.SaleDateUtc,
            s.Items.Select(i => new SaleItemResponse(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.Total)).ToList(),
            s.Total)).ToList();

        return new PagedResponse<SaleResponse>(
            items,
            input.Page,
            input.PageSize,
            result.TotalItems,
            (int)Math.Ceiling(result.TotalItems / (double)input.PageSize));
    }
}