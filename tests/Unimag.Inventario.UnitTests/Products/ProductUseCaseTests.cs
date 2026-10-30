using Unimag.Inventario.Application.Products.Commands.CreateProduct;
using Unimag.Inventario.Application.Products.Commands.DeleteProduct;
using Unimag.Inventario.Application.Products.Commands.SetProductImage;
using Unimag.Inventario.Application.Products.Commands.UpdateProduct;
using Unimag.Inventario.Application.Products.Queries.GetProductById;
using Unimag.Inventario.Application.Products.Queries.ListProducts;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.UnitTests.Fakes;

namespace Unimag.Inventario.UnitTests.Products;

public class ProductUseCaseTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public async Task CreateProduct_ReturnsCreated()
    {
        var repo = new InMemoryProductRepository();
        var useCase = new CreateProductUseCase(repo);

        var result = await useCase.ExecuteAsync(
            new CreateProductCommand("Coca", "Bebida", 2.5m, 10, CategoryId, "http://img"), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Coca", result.Name);
        Assert.Equal(2.5m, result.Price);
        Assert.Equal(10, result.Stock);
        Assert.Equal(CategoryId, result.CategoryId);
        Assert.Equal("http://img", result.ImageUrl);
    }

    [Fact]
    public async Task CreateProduct_InvalidPrice_Throws()
    {
        var repo = new InMemoryProductRepository();
        var useCase = new CreateProductUseCase(repo);

        await Assert.ThrowsAsync<InvalidPriceException>(() =>
            useCase.ExecuteAsync(new CreateProductCommand("X", null, -1m, 0, CategoryId, null), CancellationToken.None));
    }

    [Fact]
    public async Task CreateProduct_EmptyName_Throws()
    {
        var repo = new InMemoryProductRepository();
        var useCase = new CreateProductUseCase(repo);

        await Assert.ThrowsAsync<ProductNameRequiredException>(() =>
            useCase.ExecuteAsync(new CreateProductCommand("  ", null, 1m, 0, CategoryId, null), CancellationToken.None));
    }

    [Fact]
    public async Task GetProductById_NotFound_Throws()
    {
        var repo = new InMemoryProductRepository();
        var useCase = new GetProductByIdUseCase(repo);

        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            useCase.ExecuteAsync(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProduct_UpdatesFields()
    {
        var repo = new InMemoryProductRepository();
        var create = new CreateProductUseCase(repo);
        var created = await create.ExecuteAsync(new CreateProductCommand("Old", null, 1m, 5, CategoryId, null), CancellationToken.None);

        var update = new UpdateProductUseCase(repo);
        var updated = await update.ExecuteAsync(new UpdateProductCommand(created.Id, "New", "desc", 2m, 8, CategoryId), CancellationToken.None);

        Assert.Equal("New", updated.Name);
        Assert.Equal(2m, updated.Price);
        Assert.Equal(8, updated.Stock);
    }

    [Fact]
    public async Task SetProductImage_UpdatesImage()
    {
        var repo = new InMemoryProductRepository();
        var create = new CreateProductUseCase(repo);
        var created = await create.ExecuteAsync(new CreateProductCommand("P", null, 1m, 1, CategoryId, null), CancellationToken.None);

        var setImage = new SetProductImageUseCase(repo);
        var result = await setImage.ExecuteAsync(new SetProductImageCommand(created.Id, "http://new"), CancellationToken.None);

        Assert.Equal("http://new", result.ImageUrl);
    }

    [Fact]
    public async Task DeleteProduct_Removes()
    {
        var repo = new InMemoryProductRepository();
        var create = new CreateProductUseCase(repo);
        var created = await create.ExecuteAsync(new CreateProductCommand("P", null, 1m, 1, CategoryId, null), CancellationToken.None);

        var delete = new DeleteProductUseCase(repo);
        var ok = await delete.ExecuteAsync(new DeleteProductCommand(created.Id), CancellationToken.None);

        Assert.True(ok);
        var list = new ListProductsUseCase(repo);
        var all = await list.ExecuteAsync(new ListProductsQuery(), CancellationToken.None);
        Assert.Empty(all);
    }

    [Fact]
    public async Task ListProducts_FiltersByCategoryAndSearch()
    {
        var repo = new InMemoryProductRepository();
        var create = new CreateProductUseCase(repo);
        var cat2 = Guid.NewGuid();
        await create.ExecuteAsync(new CreateProductCommand("Coca", null, 1m, 1, CategoryId, null), CancellationToken.None);
        await create.ExecuteAsync(new CreateProductCommand("Pepsi", null, 1m, 1, cat2, null), CancellationToken.None);

        var list = new ListProductsUseCase(repo);
        var filtered = await list.ExecuteAsync(new ListProductsQuery(CategoryId, "co"), CancellationToken.None);

        Assert.Single(filtered);
        Assert.Equal("Coca", filtered[0].Name);
    }
}