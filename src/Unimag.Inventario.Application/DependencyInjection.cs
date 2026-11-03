using Microsoft.Extensions.DependencyInjection;
using Unimag.Inventario.Application.Categories.Commands.CreateCategory;
using Unimag.Inventario.Application.Categories.Commands.DeleteCategory;
using Unimag.Inventario.Application.Categories.Commands.UpdateCategory;
using Unimag.Inventario.Application.Categories.Dtos;
using Unimag.Inventario.Application.Categories.Queries.GetCategoryById;
using Unimag.Inventario.Application.Categories.Queries.ListCategories;
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
using Unimag.Inventario.Application.Auth.Commands.Login;
using Unimag.Inventario.Application.Auth.Dtos;
using Unimag.Inventario.Application.Sales.Queries.GetSalesSummary;
using Unimag.Inventario.Application.Sales.Queries.ListSales;
using Unimag.Inventario.Application.Tenants.Commands.CreateTenant;
using Unimag.Inventario.Application.Tenants.Commands.DeleteTenant;
using Unimag.Inventario.Application.Tenants.Commands.UpdateTenantBranding;
using Unimag.Inventario.Application.Tenants.Dtos;
using Unimag.Inventario.Application.Tenants.Queries.GetTenantById;
using Unimag.Inventario.Application.Tenants.Queries.ListTenants;
using Unimag.Inventario.Application.Users.Commands.RegisterUser;
using Unimag.Inventario.Application.Users.Dtos;
using Unimag.Inventario.Application.Users.Queries.GetUserById;
using Unimag.Inventario.Application.Platform;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Common.Dtos;
using Unimag.Inventario.Application.InventoryMovements.Commands.CreateInventoryMovement;
using Unimag.Inventario.Application.InventoryMovements.Dtos;
using Unimag.Inventario.Application.InventoryMovements.Queries.GetInventoryMovementById;
using Unimag.Inventario.Application.InventoryMovements.Queries.ListInventoryMovements;
using Unimag.Inventario.Application.Products.Queries.ListProductAlerts;
using Unimag.Inventario.Application.Products.Queries.GetProductByBarcode;
using Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;
using Unimag.Inventario.Application.Suppliers.Commands.UpdateSupplier;
using Unimag.Inventario.Application.Suppliers.Commands.DeleteSupplier;
using Unimag.Inventario.Application.Suppliers.Dtos;
using Unimag.Inventario.Application.Suppliers.Queries.GetSupplierById;
using Unimag.Inventario.Application.Suppliers.Queries.ListSuppliers;
using Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;
using Unimag.Inventario.Application.Purchases.Commands.ChangePurchaseStatus;
using Unimag.Inventario.Application.Purchases.Dtos;
using Unimag.Inventario.Application.Purchases.Queries.GetPurchaseById;
using Unimag.Inventario.Application.Purchases.Queries.ListPurchases;
using Unimag.Inventario.Application.Reports.Queries.GenerateSalesCsv;
using Unimag.Inventario.Application.Reports.Queries.GenerateInventoryCsv;

namespace Unimag.Inventario.Application;

/// <summary>
/// Composition entry point for this layer. Api wires this up without ever
/// knowing about the concrete use case classes.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

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
        services.AddScoped<IUseCase<ListProductsQuery, PagedResponse<ProductResponse>>, ListProductsUseCase>();

        services.AddScoped<IUseCase<CreateSaleCommand, SaleResponse>, CreateSaleUseCase>();
        services.AddScoped<IUseCase<GetSaleByIdQuery, SaleResponse>, GetSaleByIdUseCase>();
        services.AddScoped<IUseCase<GetSalesSummaryQuery, SalesSummaryResponse>, GetSalesSummaryUseCase>();
        services.AddScoped<IUseCase<ListSalesQuery, PagedResponse<SaleResponse>>, ListSalesUseCase>();

        services.AddScoped<IUseCase<CreateTenantCommand, TenantResponse>, CreateTenantUseCase>();
        services.AddScoped<IUseCase<UpdateTenantBrandingCommand, TenantResponse>, UpdateTenantBrandingUseCase>();
        services.AddScoped<IUseCase<DeleteTenantCommand, bool>, DeleteTenantUseCase>();
        services.AddScoped<IUseCase<GetTenantByIdQuery, TenantResponse>, GetTenantByIdUseCase>();
        services.AddScoped<IUseCase<ListTenantsQuery, IReadOnlyList<TenantResponse>>, ListTenantsUseCase>();

        services.AddScoped<IUseCase<RegisterUserCommand, UserResponse>, RegisterUserUseCase>();
        services.AddScoped<IUseCase<GetUserByIdQuery, UserResponse>, GetUserByIdUseCase>();

        services.AddScoped<IUseCase<LoginCommand, LoginResponse>, LoginUseCase>();
        services.AddScoped<IPlatformManagementService, PlatformManagementService>();
        services.AddScoped<IUseCase<CreateInventoryMovementCommand, InventoryMovementResponse>, CreateInventoryMovementUseCase>();
        services.AddScoped<IUseCase<GetInventoryMovementByIdQuery, InventoryMovementResponse>, GetInventoryMovementByIdUseCase>();
        services.AddScoped<IUseCase<ListInventoryMovementsQuery, PagedResponse<InventoryMovementResponse>>, ListInventoryMovementsUseCase>();
        services.AddScoped<IUseCase<ListProductAlertsQuery, IReadOnlyList<ProductAlertResponse>>, ListProductAlertsUseCase>();
        services.AddScoped<IUseCase<GetProductByBarcodeQuery, ProductResponse>, GetProductByBarcodeUseCase>();
        services.AddScoped<IUseCase<CreateSupplierCommand, SupplierResponse>, CreateSupplierUseCase>();
        services.AddScoped<IUseCase<UpdateSupplierCommand, SupplierResponse>, UpdateSupplierUseCase>();
        services.AddScoped<IUseCase<DeleteSupplierCommand, bool>, DeleteSupplierUseCase>();
        services.AddScoped<IUseCase<GetSupplierByIdQuery, SupplierResponse>, GetSupplierByIdUseCase>();
        services.AddScoped<IUseCase<ListSuppliersQuery, IReadOnlyList<SupplierResponse>>, ListSuppliersUseCase>();
        services.AddScoped<IUseCase<CreatePurchaseCommand, PurchaseResponse>, CreatePurchaseUseCase>();
        services.AddScoped<IUseCase<ChangePurchaseStatusCommand, PurchaseResponse>, ChangePurchaseStatusUseCase>();
        services.AddScoped<IUseCase<GetPurchaseByIdQuery, PurchaseResponse>, GetPurchaseByIdUseCase>();
        services.AddScoped<IUseCase<ListPurchasesQuery, PagedResponse<PurchaseResponse>>, ListPurchasesUseCase>();
        services.AddScoped<IUseCase<GenerateSalesCsvQuery, byte[]>, GenerateSalesCsvUseCase>();
        services.AddScoped<IUseCase<GenerateInventoryCsvQuery, byte[]>, GenerateInventoryCsvUseCase>();

        return services;
    }
}
