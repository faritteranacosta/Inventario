using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Categories.Mappers;
using Unimag.Inventario.Api.Categories.Requests;
using Unimag.Inventario.Application.Categories.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Categories.Commands.CreateCategory;
using Unimag.Inventario.Application.Categories.Commands.DeleteCategory;
using Unimag.Inventario.Application.Categories.Commands.UpdateCategory;
using Unimag.Inventario.Application.Categories.Queries.GetCategoryById;
using Unimag.Inventario.Application.Categories.Queries.ListCategories;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Api.Categories;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CategoriesController : ControllerBase
{
    private readonly IUseCase<CreateCategoryCommand, CategoryResponse> _createCategory;
    private readonly IUseCase<UpdateCategoryCommand, CategoryResponse> _updateCategory;
    private readonly IUseCase<DeleteCategoryCommand, bool> _deleteCategory;
    private readonly IUseCase<GetCategoryByIdQuery, CategoryResponse> _getCategoryById;
    private readonly IUseCase<ListCategoriesQuery, IReadOnlyList<CategoryResponse>> _listCategories;
    private readonly ICurrentTenant _currentTenant;

    public CategoriesController(
        IUseCase<CreateCategoryCommand, CategoryResponse> createCategory,
        IUseCase<UpdateCategoryCommand, CategoryResponse> updateCategory,
        IUseCase<DeleteCategoryCommand, bool> deleteCategory,
        IUseCase<GetCategoryByIdQuery, CategoryResponse> getCategoryById,
        IUseCase<ListCategoriesQuery, IReadOnlyList<CategoryResponse>> listCategories,
        ICurrentTenant currentTenant)
    {
        _createCategory = createCategory;
        _updateCategory = updateCategory;
        _deleteCategory = deleteCategory;
        _getCategoryById = getCategoryById;
        _listCategories = listCategories;
        _currentTenant = currentTenant;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = CategoryMapper.ToCommand(_currentTenant.TenantId, request);
        var result = await _createCategory.ExecuteAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> List(CancellationToken cancellationToken)
    {
        var result = await _listCategories.ExecuteAsync(CategoryMapper.ToListQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getCategoryById.ExecuteAsync(CategoryMapper.ToQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<ActionResult<CategoryResponse>> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = CategoryMapper.ToCommand(id, request);
        var result = await _updateCategory.ExecuteAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,PlatformAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _deleteCategory.ExecuteAsync(CategoryMapper.ToDeleteCommand(id), cancellationToken);
        return NoContent();
    }
}