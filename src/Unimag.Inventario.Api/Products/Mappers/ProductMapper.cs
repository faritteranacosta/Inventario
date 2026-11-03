using Unimag.Inventario.Api.Products.Requests;
using Unimag.Inventario.Application.Products.Commands.CreateProduct;
using Unimag.Inventario.Application.Products.Commands.DeleteProduct;
using Unimag.Inventario.Application.Products.Commands.SetProductImage;
using Unimag.Inventario.Application.Products.Commands.UpdateProduct;
using Unimag.Inventario.Application.Products.Queries.GetProductById;
using Unimag.Inventario.Application.Products.Queries.ListProducts;
using Unimag.Inventario.Application.Products.Queries.GetProductByBarcode;

namespace Unimag.Inventario.Api.Products.Mappers;

public static class ProductMapper
{
    public static CreateProductCommand ToCreateCommand(Guid tenantId, CreateProductRequest request) =>
        new(tenantId, request.Name, request.Description, request.Price, request.Stock, request.CategoryId, request.ImageUrl, request.MinimumStock, request.Barcode);

    public static UpdateProductCommand ToUpdateCommand(Guid id, UpdateProductRequest request) =>
        new(id, request.Name, request.Description, request.Price, request.Stock, request.CategoryId, request.MinimumStock, request.Barcode);

    public static DeleteProductCommand ToDeleteCommand(Guid id) => new(id);

    public static SetProductImageCommand ToSetImageCommand(Guid id, SetProductImageRequest request) =>
        new(id, request.ImageUrl);

    public static GetProductByIdQuery ToGetQuery(Guid id) => new(id);

    public static ListProductsQuery ToListQuery(Guid? categoryId, string? search, int page, int pageSize) =>
        new(categoryId, search, page, pageSize);

    public static GetProductByBarcodeQuery ToBarcodeQuery(string barcode) => new(barcode);
}