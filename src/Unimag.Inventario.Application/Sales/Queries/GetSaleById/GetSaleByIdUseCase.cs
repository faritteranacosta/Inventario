using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Sales.Contracts;
using Unimag.Inventario.Domain.Sales.Exceptions;

namespace Unimag.Inventario.Application.Sales.Queries.GetSaleById;

public sealed class GetSaleByIdUseCase : IUseCase<GetSaleByIdQuery, SaleResponse>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleByIdUseCase(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<SaleResponse> ExecuteAsync(GetSaleByIdQuery input, CancellationToken cancellationToken = default)
    {
        var sale = await _saleRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new SaleNotFoundException(input.Id);

        return new SaleResponse(sale.Id, sale.SaleDateUtc,
            sale.Items.Select(i => new SaleItemResponse(i.ProductId, i.Quantity, i.UnitPrice, i.Total)).ToList(),
            sale.Total);
    }
}