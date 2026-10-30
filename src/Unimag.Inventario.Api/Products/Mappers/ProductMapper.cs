using Unimag.Inventario.Api.Products.Requests;
using Unimag.Inventario.Application.Products.Commands.CreateProduct;
using Unimag.Inventario.Application.Products.Commands.DeleteProduct;
using Unimag.Inventario.Application.Products.Commands.SetProductImage;
using Unimag.Inventario.Application.Products.Commands.UpdateProduct;
using Unimag.Inventario.Application.Products.Queries.GetProductById;
using Unimag.Inventario.Application.Products.Queries.ListProducts;

namespace Unimag.Inventario.Api.Products.Mappers;

public static class ProductMapper
{
    public static CreateProductCommand ToCreateCommand(CreateProductRequest request) =>
        new(request.Name, request.Description, request.Price, request.Stock, request.CategoryId, request.ImageUrl);

    public static UpdateProductCommand ToUpdateCommand(Guid id, UpdateProductRequest request) =>
        new(id, request.Name, request.Description, request.Price, request.Stock, request.CategoryId);

    public static DeleteProductCommand ToDeleteCommand(Guid id) => new(id);

    public static SetProductImageCommand ToSetImageCommand(Guid id, SetProductImageRequest request) =>
        new(id, request.ImageUrl);

    public static GetProductByIdQuery ToGetQuery(Guid id) => new(id);

    public static ListProductsQuery ToListQuery(Guid? categoryId, string? search) =>
        new(categoryId, search);
}