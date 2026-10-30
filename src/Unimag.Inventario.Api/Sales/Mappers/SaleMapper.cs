using Unimag.Inventario.Api.Sales.Requests;
using Unimag.Inventario.Application.Sales.Commands.CreateSale;
using Unimag.Inventario.Application.Sales.Queries.GetSaleById;
using Unimag.Inventario.Application.Sales.Queries.ListSales;

namespace Unimag.Inventario.Api.Sales.Mappers;

public static class SaleMapper
{
    public static CreateSaleCommand ToCreateCommand(CreateSaleRequest request) =>
        new(request.Items.Select(i => (i.ProductId, i.Quantity)).ToList());

    public static GetSaleByIdQuery ToGetQuery(Guid id) => new(id);

    public static ListSalesQuery ToListQuery(DateTime? from, DateTime? to) =>
        new(from, to);
}