using Microsoft.Extensions.DependencyInjection;
using Unimag.Inventario.Application.Categories.Commands.CreateCategory;
using Unimag.Inventario.Application.Categories.Commands.DeleteCategory;
using Unimag.Inventario.Application.Categories.Commands.UpdateCategory;
using Unimag.Inventario.Application.Categories.Dtos;
using Unimag.Inventario.Application.Categories.Queries.GetCategoryById;
using Unimag.Inventario.Application.Categories.Queries.ListCategories;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Products.Commands.CreateProduct;
using Unimag.Inventario.Application.Products.Commands.DeleteProduct;
using Unimag.Inventario.Application.Products.Commands.SetProductImage;
using Unimag.Inventario.Application.Products.Commands.UpdateProduct;
using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Products.Queries.GetProductById;
using Unimag.Inventario.Application.Products.Queries.ListProducts;
using Unimag.Inventario.Application.Sales.Commands.CreateSale;
using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Sales.Queries.GetSaleById;
using Unimag.Inventario.Application.Sales.Queries.ListSales;
using Unimag.Inventario.Application.Samples.Commands.CreateSample;
using Unimag.Inventario.Application.Samples.Dtos;
using Unimag.Inventario.Application.Samples.Queries.GetSampleById;
using Unimag.Inventario.Application.Samples.Queries.ListSamples;

namespace Unimag.Inventario.Application;

/// <summary>
/// Composition entry point for this layer. Api wires this up without ever
/// knowing about the concrete use case classes.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUseCase<CreateSampleCommand, SampleResponse>, CreateSampleUseCase>();
        services.AddScoped<IUseCase<GetSampleByIdQuery, SampleResponse>, GetSampleByIdUseCase>();
        services.AddScoped<IUseCase<ListSamplesQuery, IReadOnlyList<SampleResponse>>, ListSamplesUseCase>();

        services.AddScoped<IUseCase<CreateCategoryCommand, CategoryResponse>, CreateCategoryUseCase>();
        services.AddScoped<IUseCase<UpdateCategoryCommand, CategoryResponse>, UpdateCategoryUseCase>();
        services.AddScoped<IUseCase<DeleteCategoryCommand, bool>, DeleteCategoryUseCase>();
        services.AddScoped<IUseCase<GetCategoryByIdQuery, CategoryResponse>, GetCategoryByIdUseCase>();
        services.AddScoped<IUseCase<ListCategoriesQuery, IReadOnlyList<CategoryResponse>>, ListCategoriesUseCase>();

        services.AddScoped<IUseCase<CreateProductCommand, ProductResponse>, CreateProductUseCase>();
        services.AddScoped<IUseCase<UpdateProductCommand, ProductResponse>, UpdateProductUseCase>();
        services.AddScoped<IUseCase<DeleteProductCommand, bool>, DeleteProductUseCase>();
        services.AddScoped<IUseCase<SetProductImageCommand, ProductResponse>, SetProductImageUseCase>();
        services.AddScoped<IUseCase<GetProductByIdQuery, ProductResponse>, GetProductByIdUseCase>();
        services.AddScoped<IUseCase<ListProductsQuery, IReadOnlyList<ProductResponse>>, ListProductsUseCase>();

        services.AddScoped<IUseCase<CreateSaleCommand, SaleResponse>, CreateSaleUseCase>();
        services.AddScoped<IUseCase<GetSaleByIdQuery, SaleResponse>, GetSaleByIdUseCase>();
        services.AddScoped<IUseCase<ListSalesQuery, IReadOnlyList<SaleResponse>>, ListSalesUseCase>();

        return services;
    }
}
