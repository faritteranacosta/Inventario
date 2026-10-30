using Unimag.Inventario.Application.Categories.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Categories.Contracts;
using Unimag.Inventario.Domain.Categories.Exceptions;

namespace Unimag.Inventario.Application.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdUseCase : IUseCase<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> ExecuteAsync(GetCategoryByIdQuery input, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new CategoryNotFoundException(input.Id);

        return new CategoryResponse(category.Id, category.Name, category.Description, category.CreatedAtUtc);
    }
}