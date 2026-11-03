using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Sales.Contracts;
using Unimag.Inventario.Domain.Sales.Exceptions;
using Unimag.Inventario.Domain.Tenants.Contracts;

namespace Unimag.Inventario.Application.Sales.Queries.GetSaleById;

public sealed class GetSaleByIdUseCase : IUseCase<GetSaleByIdQuery, SaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ITenantRepository _tenantRepository;

    public GetSaleByIdUseCase(ISaleRepository saleRepository, ITenantRepository tenantRepository)
    {
        _saleRepository = saleRepository;
        _tenantRepository = tenantRepository;
    }

    public async Task<SaleResponse> ExecuteAsync(GetSaleByIdQuery input, CancellationToken cancellationToken = default)
    {
        var sale = await _saleRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new SaleNotFoundException(input.Id);

        var tenant = await _tenantRepository.GetByIdAsync(sale.TenantId, cancellationToken);
        var tenantName = tenant?.Name ?? sale.TenantName;

        return new SaleResponse(sale.Id, tenantName, sale.SaleDateUtc,
            sale.Items.Select(i => new SaleItemResponse(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.Total)).ToList(),
            sale.Total);
    }
}