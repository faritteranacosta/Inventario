using Unimag.Inventario.Api.Sales.Requests;
using Unimag.Inventario.Application.Sales.Commands.CreateSale;
using Unimag.Inventario.Application.Sales.Queries.GetSaleById;
using Unimag.Inventario.Application.Sales.Queries.ListSales;

namespace Unimag.Inventario.Api.Sales.Mappers;

public static class SaleMapper
{
    public static CreateSaleCommand ToCreateCommand(Guid tenantId, CreateSaleRequest request, string? idempotencyKey) =>
        new(tenantId, request.Items.Select(i => (i.ProductId, i.Quantity)).ToList(), idempotencyKey);

    public static GetSaleByIdQuery ToGetQuery(Guid id) => new(id);

    public static ListSalesQuery ToListQuery(DateTime? from, DateTime? to, int page, int pageSize) =>
        new(from, to, page, pageSize);
}