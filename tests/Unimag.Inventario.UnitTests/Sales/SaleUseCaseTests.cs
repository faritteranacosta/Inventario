using Unimag.Inventario.Application.Products.Commands.CreateProduct;
using Unimag.Inventario.Application.Products.Queries.GetProductById;
using Unimag.Inventario.Application.Sales.Commands.CreateSale;
using Unimag.Inventario.Application.Sales.Queries.GetSaleById;
using Unimag.Inventario.Application.Sales.Queries.ListSales;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Sales.Exceptions;
using Unimag.Inventario.UnitTests.Fakes;

namespace Unimag.Inventario.UnitTests.Sales;

public class SaleUseCaseTests
{
    private readonly FakeCurrentTenant _tenant = new();
    private static readonly Guid CategoryId = Guid.NewGuid();

    private async Task<Guid> CreateProductAsync(InMemoryProductRepository repo, int stock = 10, decimal price = 2.5m)
    {
        var useCase = new CreateProductUseCase(repo);
        var p = await useCase.ExecuteAsync(new CreateProductCommand(_tenant.TenantId, "Prod", null, price, stock, CategoryId, null), CancellationToken.None);
        return p.Id;
    }

    [Fact]
    public async Task CreateSale_DecrementsStockAndReturnsSale()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);
        var productId = await CreateProductAsync(productRepo, 10, 2.5m);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo);
        var result = await useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, new[] { (productId, 3) }), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Single(result.Items);
        Assert.Equal(3, result.Items[0].Quantity);
        Assert.Equal(7.5m, result.Total);

        // stock decremented
        var get = new GetProductByIdUseCase(productRepo);
        var product = await get.ExecuteAsync(new GetProductByIdQuery(productId), CancellationToken.None);
        Assert.Equal(7, product.Stock);
    }

    [Fact]
    public async Task CreateSale_InsufficientStock_Throws()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);
        var productId = await CreateProductAsync(productRepo, 2, 2.5m);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo);
        await Assert.ThrowsAsync<InsufficientStockException>(() =>
            useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, new[] { (productId, 5) }), CancellationToken.None));
    }

    [Fact]
    public async Task CreateSale_EmptyItems_Throws()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo);
        await Assert.ThrowsAsync<EmptySaleException>(() =>
            useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, Array.Empty<(Guid, int)>()), CancellationToken.None));
    }

    [Fact]
    public async Task CreateSale_ProductNotFound_Throws()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo);
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, new[] { (Guid.NewGuid(), 1) }), CancellationToken.None));
    }

    [Fact]
    public async Task GetSaleById_NotFound_Throws()
    {
        var saleRepo = new InMemorySaleRepository(_tenant);
        var useCase = new GetSaleByIdUseCase(saleRepo);

        await Assert.ThrowsAsync<SaleNotFoundException>(() =>
            useCase.ExecuteAsync(new GetSaleByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task ListSales_FiltersByDateRange()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);
        var productId = await CreateProductAsync(productRepo, 10, 1m);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo);
        await useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, new[] { (productId, 1) }), CancellationToken.None);

        var list = new ListSalesUseCase(saleRepo);
        var now = DateTime.UtcNow;
        var all = await list.ExecuteAsync(new ListSalesQuery(now.AddMinutes(-5), now.AddMinutes(5)), CancellationToken.None);

        Assert.Single(all);
    }
}