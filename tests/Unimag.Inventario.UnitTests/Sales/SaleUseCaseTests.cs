using Unimag.Inventario.Application.Products.Commands.CreateProduct;
using Unimag.Inventario.Application.Products.Queries.GetProductById;
using Unimag.Inventario.Application.Sales.Commands.CreateSale;
using Unimag.Inventario.Application.Sales.Queries.GetSaleById;
using Unimag.Inventario.Application.Sales.Queries.ListSales;
using Unimag.Inventario.Application.Reports.Queries.GenerateSalesCsv;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Sales.Exceptions;
using Unimag.Inventario.Domain.Common.Exceptions;
using Unimag.Inventario.UnitTests.Fakes;

namespace Unimag.Inventario.UnitTests.Sales;

public class SaleUseCaseTests
{
    private readonly FakeCurrentTenant _tenant = new();
    private readonly InMemoryTenantRepository _tenantRepo = new();
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

        var useCase = new CreateSaleUseCase(productRepo, saleRepo, _tenantRepo);
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

        var useCase = new CreateSaleUseCase(productRepo, saleRepo, _tenantRepo);
        await Assert.ThrowsAsync<InsufficientStockException>(() =>
            useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, new[] { (productId, 5) }), CancellationToken.None));
    }

    [Fact]
    public async Task CreateSale_EmptyItems_Throws()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo, _tenantRepo);
        await Assert.ThrowsAsync<EmptySaleException>(() =>
            useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, Array.Empty<(Guid, int)>()), CancellationToken.None));
    }

    [Fact]
    public async Task CreateSale_ProductNotFound_Throws()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo, _tenantRepo);
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, new[] { (Guid.NewGuid(), 1) }), CancellationToken.None));
    }

    [Fact]
    public async Task GetSaleById_NotFound_Throws()
    {
        var saleRepo = new InMemorySaleRepository(_tenant);
        var useCase = new GetSaleByIdUseCase(saleRepo, _tenantRepo);

        await Assert.ThrowsAsync<SaleNotFoundException>(() =>
            useCase.ExecuteAsync(new GetSaleByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task ListSales_FiltersByDateRange()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);
        var productId = await CreateProductAsync(productRepo, 10, 1m);

        var useCase = new CreateSaleUseCase(productRepo, saleRepo, _tenantRepo);
        await useCase.ExecuteAsync(new CreateSaleCommand(_tenant.TenantId, new[] { (productId, 1) }), CancellationToken.None);

        var list = new ListSalesUseCase(saleRepo);
        var now = DateTime.UtcNow;
        var all = await list.ExecuteAsync(new ListSalesQuery(now.AddMinutes(-5), now.AddMinutes(5)), CancellationToken.None);

        Assert.Single(all);
    }

    [Fact]
    public async Task CreateSale_SameIdempotencyKey_ReplaysOriginalSale()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);
        var productId = await CreateProductAsync(productRepo, 10, 2.5m);
        var useCase = new CreateSaleUseCase(productRepo, saleRepo, _tenantRepo);
        var command = new CreateSaleCommand(_tenant.TenantId, new[] { (productId, 2) }, "sale-1");

        var first = await useCase.ExecuteAsync(command, CancellationToken.None);
        var replay = await useCase.ExecuteAsync(command, CancellationToken.None);

        Assert.Equal(first.Id, replay.Id);
        var product = await new GetProductByIdUseCase(productRepo)
            .ExecuteAsync(new GetProductByIdQuery(productId), CancellationToken.None);
        Assert.Equal(8, product.Stock);
    }

    [Fact]
    public async Task ListSales_InvalidPageSize_Throws()
    {
        var useCase = new ListSalesUseCase(new InMemorySaleRepository(_tenant));

        await Assert.ThrowsAsync<InvalidPaginationException>(() =>
            useCase.ExecuteAsync(new ListSalesQuery(PageSize: 101), CancellationToken.None));
    }

    [Fact]
    public async Task GenerateSalesCsv_EscapesFormulaValues()
    {
        var productRepo = new InMemoryProductRepository(_tenant);
        var saleRepo = new InMemorySaleRepository(_tenant);
        var product = await new CreateProductUseCase(productRepo).ExecuteAsync(
            new CreateProductCommand(_tenant.TenantId, "=SUM(A1:A2)", null, 1m, 2, CategoryId, null),
            CancellationToken.None);
        await new CreateSaleUseCase(productRepo, saleRepo, _tenantRepo).ExecuteAsync(
            new CreateSaleCommand(_tenant.TenantId, new[] { (product.Id, 1) }),
            CancellationToken.None);

        var csv = await new GenerateSalesCsvUseCase(saleRepo)
            .ExecuteAsync(new GenerateSalesCsvQuery(), CancellationToken.None);
        var content = System.Text.Encoding.UTF8.GetString(csv);

        Assert.Contains("'=SUM(A1:A2)", content);
    }
}