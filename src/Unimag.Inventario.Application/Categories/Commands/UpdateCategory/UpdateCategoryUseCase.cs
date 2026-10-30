using Unimag.Inventario.Application.Categories.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Categories;
using Unimag.Inventario.Domain.Categories.Contracts;
using Unimag.Inventario.Domain.Categories.Exceptions;

namespace Unimag.Inventario.Application.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryUseCase : IUseCase<UpdateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> ExecuteAsync(UpdateCategoryCommand input, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new CategoryNotFoundException(input.Id);

        category.Rename(input.Name, input.Description);
        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(category.Id, category.Name, category.Description, category.CreatedAtUtc);
    }
}