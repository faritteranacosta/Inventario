using Unimag.Inventario.Application.Categories.Commands.CreateCategory;
using Unimag.Inventario.Application.Categories.Commands.DeleteCategory;
using Unimag.Inventario.Application.Categories.Commands.UpdateCategory;
using Unimag.Inventario.Application.Categories.Queries.GetCategoryById;
using Unimag.Inventario.Application.Categories.Queries.ListCategories;
using Unimag.Inventario.Domain.Categories.Exceptions;
using Unimag.Inventario.UnitTests.Fakes;

namespace Unimag.Inventario.UnitTests.Categories;

public class CategoryUseCaseTests
{
    [Fact]
    public async Task CreateCategory_ReturnsCreatedCategory()
    {
        var repo = new InMemoryCategoryRepository();
        var useCase = new CreateCategoryUseCase(repo);

        var result = await useCase.ExecuteAsync(new CreateCategoryCommand("Bebidas", "Liquidos"), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Bebidas", result.Name);
        Assert.Equal("Liquidos", result.Description);
    }

    [Fact]
    public async Task CreateCategory_WithEmptyName_Throws()
    {
        var repo = new InMemoryCategoryRepository();
        var useCase = new CreateCategoryUseCase(repo);

        await Assert.ThrowsAsync<CategoryNameRequiredException>(() =>
            useCase.ExecuteAsync(new CreateCategoryCommand("   ", null), CancellationToken.None));
    }

    [Fact]
    public async Task GetCategoryById_NotFound_Throws()
    {
        var repo = new InMemoryCategoryRepository();
        var useCase = new GetCategoryByIdUseCase(repo);

        await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
            useCase.ExecuteAsync(new GetCategoryByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCategory_Renames()
    {
        var repo = new InMemoryCategoryRepository();
        var create = new CreateCategoryUseCase(repo);
        var created = await create.ExecuteAsync(new CreateCategoryCommand("Old", null), CancellationToken.None);

        var update = new UpdateCategoryUseCase(repo);
        var updated = await update.ExecuteAsync(new UpdateCategoryCommand(created.Id, "New", "desc"), CancellationToken.None);

        Assert.Equal("New", updated.Name);
        Assert.Equal("desc", updated.Description);
    }

    [Fact]
    public async Task DeleteCategory_Removes()
    {
        var repo = new InMemoryCategoryRepository();
        var create = new CreateCategoryUseCase(repo);
        var created = await create.ExecuteAsync(new CreateCategoryCommand("ToDelete", null), CancellationToken.None);

        var delete = new DeleteCategoryUseCase(repo);
        var ok = await delete.ExecuteAsync(new DeleteCategoryCommand(created.Id), CancellationToken.None);

        Assert.True(ok);
        var list = new ListCategoriesUseCase(repo);
        var all = await list.ExecuteAsync(new ListCategoriesQuery(), CancellationToken.None);
        Assert.Empty(all);
    }

    [Fact]
    public async Task ListCategories_ReturnsAll()
    {
        var repo = new InMemoryCategoryRepository();
        var create = new CreateCategoryUseCase(repo);
        await create.ExecuteAsync(new CreateCategoryCommand("A", null), CancellationToken.None);
        await create.ExecuteAsync(new CreateCategoryCommand("B", null), CancellationToken.None);

        var list = new ListCategoriesUseCase(repo);
        var all = await list.ExecuteAsync(new ListCategoriesQuery(), CancellationToken.None);

        Assert.Equal(2, all.Count);
    }
}