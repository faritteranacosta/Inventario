using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Categories.Contracts;
using Unimag.Inventario.Domain.Categories.Exceptions;

namespace Unimag.Inventario.Application.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryUseCase : IUseCase<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> ExecuteAsync(DeleteCategoryCommand input, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new CategoryNotFoundException(input.Id);

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}