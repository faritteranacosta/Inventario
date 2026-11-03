namespace Unimag.Inventario.Application.Products.Queries.ListProducts;

public sealed record ListProductsQuery(
    Guid? CategoryId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 25);