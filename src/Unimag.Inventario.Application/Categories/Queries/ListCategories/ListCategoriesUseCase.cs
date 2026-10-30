using Unimag.Inventario.Application.Categories.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Categories.Contracts;

namespace Unimag.Inventario.Application.Categories.Queries.ListCategories;

public sealed class ListCategoriesUseCase : IUseCase<ListCategoriesQuery, IReadOnlyList<CategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;

    public ListCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryResponse>> ExecuteAsync(ListCategoriesQuery input, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => new CategoryResponse(c.Id, c.Name, c.Description, c.CreatedAtUtc)).ToList();
    }
}