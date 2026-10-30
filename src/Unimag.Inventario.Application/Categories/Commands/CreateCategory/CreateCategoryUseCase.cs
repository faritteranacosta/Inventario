using Unimag.Inventario.Application.Categories.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Categories;
using Unimag.Inventario.Domain.Categories.Contracts;

namespace Unimag.Inventario.Application.Categories.Commands.CreateCategory;

public sealed class CreateCategoryUseCase : IUseCase<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> ExecuteAsync(CreateCategoryCommand input, CancellationToken cancellationToken = default)
    {
        var category = Category.Create(input.Name, input.Description);
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(category.Id, category.Name, category.Description, category.CreatedAtUtc);
    }
}